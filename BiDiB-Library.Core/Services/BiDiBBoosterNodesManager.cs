using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Models.Messages.Output;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Services;

/// <summary>
/// Internal manager for all booster related information
/// </summary>
[Export(typeof(IBiDiBBoosterNodesManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class BiDiBBoosterNodesManager : IBiDiBBoosterNodesManager
{
    private readonly ILogger<BiDiBBoosterNodesManager> logger;
    private readonly IList<BiDiBBoosterNode> boosters;
    private readonly ILoggerFactory loggerFactory;

    [ImportingConstructor]
    public BiDiBBoosterNodesManager(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<BiDiBBoosterNodesManager>();
        boosters = new List<BiDiBBoosterNode>();
        this.loggerFactory = loggerFactory;
    }

    public IEnumerable<BiDiBBoosterNode> Boosters => boosters;
    public Action<BiDiBBoosterNode> BoosterAdded { get; set; }
    public Action<BiDiBBoosterNode> BoosterRemoved { get; set; }
        
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

        var booster = new BiDiBBoosterNode(node, loggerFactory.CreateLogger<BiDiBBoosterNode>());

        var currentFeature = node.GetFeature(BiDiBFeature.FEATURE_BST_AMPERE);
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
        var booster = boosters.FirstOrDefault(x => x.Node.UniqueId == removedNode.UniqueId);
        if (booster == null) { return; }

        booster.Dispose();
        boosters.Remove(booster);
        BoosterRemoved?.Invoke(booster);
    }
}