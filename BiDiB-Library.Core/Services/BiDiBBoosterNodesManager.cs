using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.BiDiB;
using org.bidib.netbidibc.core.Models.BiDiB.Extensions;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Models.Messages.Output;
using org.bidib.netbidibc.core.Services.Interfaces;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Services
{
    /// <summary>
    /// Internal manager for all booster related information
    /// </summary>
    //[Export(typeof(IBiDiBBoosterNodesManager))]
    //[Export(typeof(IMessageReceiver))]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public class BiDiBBoosterNodesManager : IBiDiBBoosterNodesManager
    {
        private readonly ILogger<BiDiBBoosterNodesManager> logger;
        private readonly IList<BiDiBBoosterNode> boosters;
        private readonly ILoggerFactory loggerFactory;

        public BiDiBBoosterNodesManager(IBiDiBMessageService messageService, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<BiDiBBoosterNodesManager>();
            boosters = new List<BiDiBBoosterNode>();
            this.loggerFactory = loggerFactory;
            messageService.Register(this);
        }

        public IEnumerable<BiDiBBoosterNode> Boosters => boosters;
        public Action<BiDiBBoosterNode> BoosterAdded { get; set; }
        public Action<BiDiBBoosterNode> BoosterRemoved { get; set; }


        public void ProcessMessage(BiDiBInputMessage message)
        {
            if (message == null) { return; }

            switch (message.MessageType)
            {
                case BiDiBMessage.MSG_BOOST_DIAGNOSTIC: { HandleBoostDiagnostic(message as BoostDiagnosticMessage); break; }
                case BiDiBMessage.MSG_BOOST_STAT: { HandleBoostStat(message as BoostStatMessage); break; }
                case BiDiBMessage.MSG_CS_STATE: { HandleCommandStationState(message as CommandStationStateMessage); break; }
            }
        }

        public void NodeAdded(BiDiBNode newNode)
        {
            CheckForNewNode(newNode);
        }

        private void CheckForNewNode(BiDiBNode node)
        {
            if (!node.HasBoosterFunctions && !node.HasCommandStationFunctions) { return; }

            if (boosters.Any(x => x.Node.UniqueId == node.UniqueId))
            {
                logger.LogWarning($"Booster item with Id '{node.UniqueId} already exists");
                return;
            }

            BiDiBBoosterNode booster = new BiDiBBoosterNode(node, loggerFactory.CreateLogger<BiDiBBoosterNode>());

            Feature currentFeature = node.GetFeature(BiDiBFeature.FEATURE_BST_AMPERE);
            if (currentFeature != null)
            {
                booster.MaxCurrent = currentFeature.Value.CalculateBoosterCurrent();
            }

            boosters.Add(booster);

            if (node.HasBoosterFunctions)
            {
                node.MessageProcessor.SendMessage(node, BiDiBMessage.MSG_BOOST_QUERY);
            }

            if (node.HasCommandStationFunctions)
            {
                node.MessageProcessor.SendMessage(new CommandStationSetStateMessage(CommandStationState.BIDIB_CS_STATE_QUERY, node.Address));
            }

            BoosterAdded?.Invoke(booster);
        }

        public void NodeRemoved(BiDiBNode removedNode)
        {
            BiDiBBoosterNode booster = boosters.FirstOrDefault(x => x.Node.UniqueId == removedNode.UniqueId);
            if (booster == null) { return; }

            booster.Dispose();
            boosters.Remove(booster);
            BoosterRemoved?.Invoke(booster);
        }

        private void HandleCommandStationState(CommandStationStateMessage message)
        {
            if (message == null) { return; }

            BiDiBBoosterNode node = GetNode(message);
            if (node == null) { return; }

            node.DccState = message.State;
        }

        private void HandleBoostStat(BoostStatMessage message)
        {
            if (message == null) { return; }

            BiDiBBoosterNode node = GetNode(message);
            if (node == null) { return; }

            node.BoosterState = message.State;
            node.BoosterControl = message.Control;
        }

        private void HandleBoostDiagnostic(BoostDiagnosticMessage message)
        {
            if (message == null) { return; }

            BiDiBBoosterNode node = GetNode(message);
            if (node == null) { return; }

            node.Voltage = message.Voltage / 1000.0; // value will be displayed as full voltage
            node.Current = message.Current;
            node.Temp = message.Temperature;
        }

        private BiDiBBoosterNode GetNode(BiDiBInputMessage message)
        {
            int addressValue = message.Address.GetArrayValue();
            return boosters.FirstOrDefault(x => x.Node.Address.GetArrayValue() == addressValue);
        }
    }
}