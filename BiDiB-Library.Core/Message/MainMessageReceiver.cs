using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core.Message;

[Export(typeof(IMessageReceiver))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class MainMessageReceiver : IMessageReceiver
{
    private readonly ILogger<MainMessageReceiver> logger;
    private readonly IBiDiBMessageService messageService;
    private readonly IBiDiBNodesFactory nodesFactory;
    private readonly Dictionary<BiDiBMessage, Action<BiDiBInputMessage>> messageHandlers;

    [ImportingConstructor]
    public MainMessageReceiver(IBiDiBMessageService messageService, IBiDiBNodesFactory nodesFactory, ILoggerFactory loggerFactory)
    {
        this.messageService = messageService;
        this.nodesFactory = nodesFactory;
        logger = loggerFactory.CreateLogger<MainMessageReceiver>();
        messageHandlers = new Dictionary<BiDiBMessage, Action<BiDiBInputMessage>>();
        RegisterMessageHandlers();
    }

    private void RegisterMessageHandlers()
    {
        messageHandlers.Add(BiDiBMessage.MSG_NODE_NEW, message => HandleNodeNew(message as NodeNewMessage));
        messageHandlers.Add(BiDiBMessage.MSG_NODE_LOST, message => HandleNodeLost(message as NodeLostMessage));
        messageHandlers.Add(BiDiBMessage.MSG_BM_MULTIPLE, message => HandleFeedbackMultiple(message as FeedbackMultipleMessage));
        messageHandlers.Add(BiDiBMessage.MSG_BM_FREE, message => HandleFeedbackMessage(message as FeedbackMessage, true));
        messageHandlers.Add(BiDiBMessage.MSG_BM_OCC, message => HandleFeedbackMessage(message as FeedbackOccupiedMessage, false));
        messageHandlers.Add(BiDiBMessage.MSG_BM_ADDRESS, message => HandleFeedbackAddressMessage(message as FeedbackAddressMessage));
        messageHandlers.Add(BiDiBMessage.MSG_BM_SPEED, message => HandleFeedbackSpeedMessage(message as FeedbackSpeedMessage));
        messageHandlers.Add(BiDiBMessage.MSG_BM_DYN_STATE, message => HandleFeedbackDynStateMessage(message as FeedbackDynStateMessage));
        messageHandlers.Add(BiDiBMessage.MSG_BM_POSITION, message => HandleFeedbackPositionMessage(message as FeedbackPositionMessage));
        messageHandlers.Add(BiDiBMessage.MSG_SYS_IDENTIFY_STATE, message => HandleIdentifyState(message as SysIdentifyStateMessage));
    }

    public void ProcessMessage(BiDiBInputMessage message)
    {
        if (message == null || !messageHandlers.TryGetValue(message.MessageType, out var handler))
        {
            return;
        }

        handler(message);
    }

    private void HandleNodeNew(NodeTabMessage message)
    {
        if (message == null) { return; }

        messageService.SendMessage(BiDiBMessage.MSG_NODE_CHANGED_ACK, message.Address, message.TableVersion);
        Task.Factory.StartNew(() => nodesFactory.CreateNode(message.NodeAddress, message.UniqueId));
    }

    private void HandleNodeLost(NodeTabMessage message)
    {
        if (message == null) { return; }

        messageService.SendMessage(BiDiBMessage.MSG_NODE_CHANGED_ACK, message.Address, message.TableVersion);
        nodesFactory.RemoveNode(message.NodeAddress);
        messageService.ResetMessageSequenceNumber(message.NodeAddress);
    }

    private void HandleFeedbackMultiple(FeedbackMultipleMessage message)
    {
        if (message == null) { return; }
        messageService.SendMessage(BiDiBMessage.MSG_BM_MIRROR_MULTIPLE, message.Address, message.MessageParameters);

        Node node = nodesFactory.GetNode(message.Address);

        if (node?.FeedbackPorts == null)
        {
            return;
        }

        for (var i = 0; i < message.StateSize; i++)
        {
            UpdateOccupancy(node, message.FeedbackNumber + i, !message.PortStates[i], message.Timestamp);
        }
    }

    private void HandleFeedbackMessage(FeedbackMessage feedbackMessage, bool isFree)
    {
        messageService.SendMessage(isFree ? BiDiBMessage.MSG_BM_MIRROR_FREE : BiDiBMessage.MSG_BM_MIRROR_OCC, feedbackMessage.Address, feedbackMessage.FeedbackNumber);

        Node node = nodesFactory.GetNode(feedbackMessage.Address);

        if (node?.FeedbackPorts == null)
        {
            return;
        }

        UpdateOccupancy(node, feedbackMessage.FeedbackNumber, isFree, feedbackMessage.Timestamp);
    }

    private void HandleFeedbackPositionMessage(FeedbackPositionMessage message)
    {
        if (message == null)
        {
            return;
        }
        
        messageService.SendMessage(BiDiBMessage.MSG_BM_MIRROR_POSITION, message.Address, message.MessageParameters);

        var node = nodesFactory.GetNode(message.Address);

        if (node == null) { return; }

        var occupiedPorts = node.PositionPorts.Values
            .Where(x => x.Occupancies != null && Array.Exists(x.Occupancies, o => o.Address == message.FeedbackAddress))
            .ToList();
        
        foreach (var occupiedPort in occupiedPorts)
        {
            occupiedPort.ClearOccupancies();
        }

        var port = node.PositionPorts.TryGetValue(message.Location, out var positionPort)
            ? positionPort
            : new PositionPort(message.Location);

        port.AddOccupancy(new OccupancyInfo { Address = message.FeedbackAddress });

        node.UpdatePosition(port);
    }

    private void HandleFeedbackAddressMessage(FeedbackAddressMessage message)
    {
        var node = nodesFactory.GetNode(message.Address);

        if (node == null) { return; }

        IOccupanciesHost occupancyProvider =
            node.FeedbackPorts == null || node.FeedbackPorts.Length <= message.FeedbackNumber
                ? node
                : node.FeedbackPorts[message.FeedbackNumber];

        if (message.Addresses.Count() == 1 && message.Addresses.ElementAt(0).Address == 0)
        {
            occupancyProvider.ClearOccupancies();
            return;
        }

        foreach (var decoderInfo in message.Addresses)
        {
            var info = occupancyProvider.GetOccupancy(decoderInfo.Address);

            if (info == null)
            {
                info = new OccupancyInfo { Address = decoderInfo.Address };
                occupancyProvider.AddOccupancy(info);
            }

            info.Direction = decoderInfo.Direction;
        }

        var oldInfos = occupancyProvider.GetOccupanciesByFilter(x => message.Addresses.All(a => a.Address != x.Address));
        foreach (var occupancy in oldInfos)
        {
            occupancyProvider.RemoveOccupancy(occupancy);
        }
    }

    private void HandleFeedbackSpeedMessage(FeedbackSpeedMessage message)
    {
        var node = nodesFactory.GetNode(message.Address);
        if (node == null) { return; }

        if (node.FeedbackPorts == null)
        {
            var occupancy = node.GetOccupancy(message.DecoderAddress);
            UpdateOccupancy(occupancy, message);
            return;
        }

        UpdateAllPortOccupancies(node, message.DecoderAddress, oi => UpdateOccupancy(oi, message));
    }

    private void HandleFeedbackDynStateMessage(FeedbackDynStateMessage message)
    {
        var node = nodesFactory.GetNode(message.Address);
        if (node == null) { return; }

        if (node.FeedbackPorts == null)
        {
            var occupancy = node.GetOccupancy(message.DecoderAddress);
            UpdateOccupancy(occupancy, message);
            return;
        }

        UpdateAllPortOccupancies(node, message.DecoderAddress, oi => UpdateOccupancy(oi, message));
    }

    private static void UpdateAllPortOccupancies(Node node, ushort decoderAddress, Action<OccupancyInfo> updateAction)
    {
        foreach (var occupancies in node.FeedbackPorts.Select(x=>x.Occupancies))
        {
            if (occupancies == null) { return; }

            foreach (var occupancyInfo in occupancies)
            {
                if (occupancyInfo.Address != decoderAddress)
                {
                    continue;
                }

                updateAction(occupancyInfo);
            }
        }
    }

    private void UpdateOccupancy(Node node, int feedbackNumber, bool isFree, ushort timestamp)
    {
        if (feedbackNumber >= node.FeedbackPorts.Length)
        {
            logger.LogWarning(
                "Feedback port '{FeedbackNumber}' is out of range ({NumberOfPorts}) of node {NodeAddress}",
                feedbackNumber, node.FeedbackPorts.Length, node.GetFullAddressString());
            return;
        }

        var port = node.FeedbackPorts[feedbackNumber];

        port.IsFree = isFree;

        if (isFree)
        {
            port.ClearOccupancies();
        }
        else
        {
            port.TimeOccupied = timestamp;
        }
    }

    private static void UpdateOccupancy(OccupancyInfo occupancy, FeedbackSpeedMessage message)
    {
        if (occupancy == null || message == null) { return; }
        occupancy.Speed = message.Speed;
        occupancy.Direction = message.Direction;
    }

    private void UpdateOccupancy(OccupancyInfo occupancy, FeedbackDynStateMessage message)
    {
        if (occupancy == null || message == null) { return; }
        ApplyStateValue(message.DynState, occupancy, message.StateValue);
        occupancy.Direction = message.Direction;
    }

    private void ApplyStateValue(DynState dynState, OccupancyInfo occupancyInfo, int value)
    {
        switch (dynState)
        {
            case DynState.SignalQuality:
                occupancyInfo.Quality = value;
                break;
            case DynState.Temperature:
            case DynState.Temperature2:
            {
                SetTemperature(occupancyInfo, value);
                break;
            }
            case DynState.Container1:
                occupancyInfo.Container1 = value;
                break;
            case DynState.Container2:
                occupancyInfo.Container2 = value;
                break;
            case DynState.Container3:
                occupancyInfo.Container3 = value;
                break;
            case DynState.TrackVoltage:
                occupancyInfo.TrackVoltage = value * 0.1;
                break;
            case DynState.Reserved:
            case DynState.Distance:
            {
                // Not handled at the moment
                break;
            }
            default:
            {
                logger.LogDebug("Dyn state {DynState} is not handled to occupancy!", dynState);
                break;
            }
        }

    }

    private static void SetTemperature(OccupancyInfo occupancyInfo, int value)
    {
        occupancyInfo.Temperature = value switch
        {
            <= 127 => value,
            >= 226 and <= 255 => (sbyte)value,
            _ => occupancyInfo.Temperature
        };
    }

    private void HandleIdentifyState(SysIdentifyStateMessage message)
    {
        if (message == null) { return; }

        var node = nodesFactory.GetNode(message.Address);

        if (node == null) { return; }

        node.State = message.Enabled ? NodeState.Identifying : NodeState.Ok;
    }
}