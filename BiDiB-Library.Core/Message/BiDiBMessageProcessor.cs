using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.BiDiB.Base;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Models.Messages.Output;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Message;

[Localizable(false)]
[Export(typeof(IBiDiBMessageProcessor))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class BiDiBMessageProcessor : IBiDiBMessageProcessor
{
    private readonly ILogger<BiDiBMessageProcessor> logger;
    private readonly ILogger latencyLogger;
    private readonly IBiDiBNodesFactory nodesFactory;
    private readonly IBiDiBMessageService messageService;
    private const int ResponseTimeout = 500;

    [ImportingConstructor]
    public BiDiBMessageProcessor(IBiDiBNodesFactory nodesFactory, IBiDiBMessageService messageService, ILoggerFactory loggerFactory)
    {
        this.nodesFactory = nodesFactory;
        this.messageService = messageService;
        logger = loggerFactory.CreateLogger<BiDiBMessageProcessor>();
        latencyLogger = loggerFactory.CreateLogger(BiDiBConstants.LoggerContextLatency);
    }

    public void GetChildNodes(byte[] parentAddress)
    {
        var parentNode = nodesFactory.GetNode(parentAddress);
        if (parentNode == null) { return; }

        var nodeTabCountMessage = SendMessage<NodeTabCountMessage>(parentNode, BiDiBMessage.MSG_NODETAB_GETALL);
        if (nodeTabCountMessage == null) { return; }

        var retryCount = 0;
        while (nodeTabCountMessage.NodeCount == 0 && retryCount < 5)
        {
            logger.LogDebug("node {Address} is still initializing. Wait and try again", parentNode.FullAddress);
            Thread.Sleep(ResponseTimeout);
            nodeTabCountMessage = SendMessage<NodeTabCountMessage>(parentNode, BiDiBMessage.MSG_NODETAB_GETALL);
            retryCount++;
        }

        if (nodeTabCountMessage.NodeCount == 0)
        {
            logger.LogWarning("node {Address} is still initializing after {Retries}", parentNode.FullAddress, retryCount + 1);
            return;
        }

        for (var i = 0; i < nodeTabCountMessage.NodeCount; i++)
        {
            var nodeTabMessage = SendMessage<NodeTabMessage>(parentNode, BiDiBMessage.MSG_NODETAB_GETNEXT);
            if (nodeTabMessage == null) { continue; }

            var node = nodesFactory.GetNode(nodeTabMessage.NodeAddress)
                       ?? nodesFactory.CreateNode(nodeTabMessage.NodeAddress, nodeTabMessage.UniqueId);

            logger.LogDebug("Node detected UID: {HexUniqueId}, ADDR: {Address}, VPID: {VendorProductId}", node.HexUniqueId, node.GetFullAddressString(), node.VendorProductId);
        }
    }

    public void SendMessage(BiDiBNode node, BiDiBMessage messageType, params byte[] parameters)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }
        
        messageService.SendMessage(messageType, node.Address, parameters);
    }

    public TResponseMessage SendMessage<TResponseMessage>(BiDiBNode node, BiDiBMessage messageType, params byte[] parameters)
        where TResponseMessage : BiDiBInputMessage
    {
        return SendMessage<TResponseMessage>(node, messageType, ResponseTimeout, false, parameters);
    }
    
    public TResponseMessage SendMessage<TResponseMessage>(BiDiBNode node, BiDiBMessage messageType, int timeout, params byte[] parameters)
        where TResponseMessage : BiDiBInputMessage
    {
        return SendMessage<TResponseMessage>(node, messageType, timeout, false, parameters);
    }

    public TResponseMessage SendMessage<TResponseMessage>(BiDiBNode node, BiDiBMessage messageType, int timeout, bool acceptFromAnySender, params byte[] parameters)
        where TResponseMessage : BiDiBInputMessage
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }
        
        var messages = new List<BiDiBOutputMessage> { new(node.Address, messageType, parameters) };
        return SendMessageMultiResponseMessage<TResponseMessage>(node.Address, messages, 1, timeout, acceptFromAnySender).FirstOrDefault();
    }

    public void SendMessage(BiDiBOutputMessage outputMessage)
    {
        messageService.SendMessage(outputMessage);
    }

    public TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage)
        where TResponseMessage : BiDiBInputMessage
    {
        return SendMessage<TResponseMessage>(outputMessage, ResponseTimeout);
    }
    
    public TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage, int timeout)
        where TResponseMessage : BiDiBInputMessage
    {
        return SendMessage<TResponseMessage>(outputMessage, timeout, false);
    }

    public TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage, int timeout, bool acceptFromAnySender)
        where TResponseMessage : BiDiBInputMessage
    {
        return outputMessage == null ?
            default
            : SendMessageMultiResponseMessage<TResponseMessage>(outputMessage.Address, [outputMessage], 1, timeout, acceptFromAnySender)
                .FirstOrDefault();
    }

    public IEnumerable<TResponseMessage> SendMessages<TResponseMessage>(ICollection<BiDiBOutputMessage> outputMessages, int timeout = 300)
        where TResponseMessage : BiDiBInputMessage
    {
        if (outputMessages == null || !outputMessages.Any())
        {
            return [];
        }

        return SendMessageMultiResponseMessage<TResponseMessage>(outputMessages.First().Address, outputMessages, outputMessages.Count, timeout);
    }

    public IEnumerable<Port> GetPorts(BiDiBNode node, PortType portType, int expectedItems)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }
        
        var outputMessages = new List<BiDiBOutputMessage>
        {
            new ConfigXGetAllMessage(node.Address, portType, 0, portType, (byte)expectedItems)
        };
        var configXMessages = SendMessageMultiResponseMessage<LcConfigXMessage>(node.Address, outputMessages, expectedItems);
        return configXMessages.Select(configXMessage => GetNewPort(portType, configXMessage.PortNumber)).Where(port => port != null).ToList();
    }

    private IEnumerable<TResponseMessage> SendMessageMultiResponseMessage<TResponseMessage>(byte[] address,
        ICollection<BiDiBOutputMessage> outputMessages,
        int expectedMessages,
        int timeout = ResponseTimeout,
        bool acceptFromAnySender = false)
        where TResponseMessage : BiDiBInputMessage
    {
        var timedout = false;
        var receivedCount = 0;
        ManualResetEventSlim manualReset = new();
        List<TResponseMessage> responseMessages = [];

        RuntimeMessageReceiver<TResponseMessage> messageReceiver = new(x =>
        {
            if (!acceptFromAnySender && x.Address.GetArrayValue() != address.GetArrayValue())
            {
                logger.LogDebug("Skip processing message of other node {MessageAddress} -> {Address2}",
                    NodeExtensions.GetFullAddressString(x.Address),
                    NodeExtensions.GetFullAddressString(address));
                return;
            }

            responseMessages.Add(x);
            manualReset.Set();
        });

        messageService.Register(messageReceiver);

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        messageService.SendMessages(outputMessages);
        while (receivedCount < expectedMessages && !timedout)
        {
            manualReset.Reset();
            if (!manualReset.Wait(timeout))
            {
                timedout = true;
                logger.LogWarning("Response timeout for {Messages} ({ResponseMessages}/{ExpectedMessages})",
                    string.Join(";", outputMessages),
                    responseMessages.Count,
                    expectedMessages);
            }

            receivedCount = responseMessages.Count;
        }

        latencyLogger.LogDebug("{OutputType,-25} - {ResponseType,-25} -> {OperationTime}ms ({OutCount}/{ReponseCount})", 
            outputMessages.First().MessageType.ToString(),
            responseMessages.Count > 0 ? responseMessages[0].MessageType.ToString() : "NO",
            stopWatch.ElapsedMilliseconds,
            outputMessages.Count,
            responseMessages.Count);
        messageService.Unregister(messageReceiver);
        return responseMessages;
    }

    private static Port GetNewPort(PortType portType, byte portNumber)
    {
        switch (portType)
        {
            case PortType.Switch: { return new OutputSwitch { Number = portNumber }; }
            case PortType.Light: { return new OutputLight { Number = portNumber }; }
            case PortType.Servo: { return new OutputServo { Number = portNumber }; }
            case PortType.Sound: { return new OutputSound { Number = portNumber }; }
            case PortType.Motor: { return new OutputMotor { Number = portNumber }; }
            case PortType.AnalogOut: { return new OutputAnalog { Number = portNumber }; }
            case PortType.Backlight: { return new OutputBacklight { Number = portNumber }; }
            case PortType.Switchpair:
                break;
            case PortType.Input: { return new InputKey { Number = portNumber }; }
            case PortType.All:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(portType), portType, null);
        }

        return null;
    }
}