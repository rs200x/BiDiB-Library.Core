using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Message;

[Export(typeof(IMessageReceiver))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class BoosterMessageReceiver : IMessageReceiver
{
    private readonly IBiDiBBoosterNodesManager boosterNodesManager;

    private readonly Dictionary<BiDiBMessage, Action<BiDiBInputMessage>> messageHandlers = new();

    [ImportingConstructor]
    public BoosterMessageReceiver(IBiDiBBoosterNodesManager boosterNodesManager)
    {
        this.boosterNodesManager = boosterNodesManager;
        RegisterMessageHandlers();
    }

    private void RegisterMessageHandlers()
    {
        messageHandlers.Add(BiDiBMessage.MSG_BOOST_DIAGNOSTIC, message => HandleBoostDiagnostic(message as BoostDiagnosticMessage));
        messageHandlers.Add(BiDiBMessage.MSG_BOOST_STAT, message => HandleBoostStat(message as BoostStatMessage));
        messageHandlers.Add(BiDiBMessage.MSG_CS_STATE, message => HandleCommandStationState(message as CommandStationStateMessage));
    }

    public void ProcessMessage(BiDiBInputMessage message)
    {
        if (message == null || !messageHandlers.ContainsKey(message.MessageType))
        {
            return;
        }

        var handler = messageHandlers[message.MessageType];
        handler(message);
    }

    private void HandleCommandStationState(CommandStationStateMessage message)
    {
        if (message == null)
        {
            return;
        }

        var node = GetNode(message);
        if (node == null)
        {
            return;
        }

        node.DccState = message.State;
    }

    private void HandleBoostStat(BoostStatMessage message)
    {
        if (message == null)
        {
            return;
        }

        var node = GetNode(message);
        if (node == null)
        {
            return;
        }

        node.BoosterState = message.State;
        node.BoosterControl = message.Control;
    }

    private void HandleBoostDiagnostic(BoostDiagnosticMessage message)
    {
        if (message == null)
        {
            return;
        }

        var node = GetNode(message);
        if (node == null)
        {
            return;
        }

        node.Voltage = message.Voltage / 1000.0; // value will be displayed as full voltage
        node.Current = message.Current;
        node.Temp = message.Temperature;
    }

    private BiDiBBoosterNode GetNode(BiDiBInputMessage message)
    {
        var addressValue = message.Address.GetArrayValue();
        return boosterNodesManager.Boosters.FirstOrDefault(x => x.Node.Address.GetArrayValue() == addressValue);
    }
}