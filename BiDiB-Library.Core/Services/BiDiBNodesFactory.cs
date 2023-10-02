using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Services;

[Export(typeof(IBiDiBNodesFactory))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class BiDiBNodesFactory : IBiDiBNodesFactory
{
    private readonly ConcurrentDictionary<int, BiDiBNode> nodes;
    private static readonly byte[] RootAddress = { 0 };
    private readonly ILogger<BiDiBNodesFactory> logger;
    private readonly ILoggerFactory loggerFactory;

    [ImportingConstructor]
    public BiDiBNodesFactory(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<BiDiBNodesFactory>();
        nodes = new ConcurrentDictionary<int, BiDiBNode>();
        this.loggerFactory = loggerFactory;
    }

    public IBiDiBMessageProcessor MessageProcessor { get; set; }

    public IEnumerable<BiDiBNode> Nodes => nodes.Values;

    public BiDiBNode GetRootNode()
    {
        var node = GetNode(RootAddress);
        return node ?? CreateNode(RootAddress, null);
    }

    public BiDiBNode GetNode(byte[] address) => nodes.ContainsKey(address.GetArrayValue()) ? nodes[address.GetArrayValue()] : null;

    public BiDiBNode CreateNode(byte[] address, byte[] uniqueId)
    {
        var node = new BiDiBNode(MessageProcessor, loggerFactory.CreateLogger<BiDiBNode>()) { Address = address };
        if (uniqueId != null) { node.SetUniqueId(uniqueId); }

        if (nodes.TryAdd(node.GetAddress(), node))
        {
            NodeAdded?.Invoke(node);
            return node;
        }

        logger.LogWarning($"node with address {NodeExtensions.GetFullAddressString(address)} could not be added");
        return null;
    }

    public Action<BiDiBNode> NodeAdded { get; set; }

    public void RemoveNode(byte[] address)
    {
        var subNodes = FindSubNodes(address);
        foreach (var subNode in subNodes)
        {
            RemoveNode(subNode.Address.GetArrayValue());
        }
        RemoveNode(address.GetArrayValue());
    }

    private IEnumerable<BiDiBNode> FindSubNodes(byte[] address)
    {
        // is root?
        if (address.GetArrayValue() == 0)
        {
            return Nodes.Where(x => x.Address.GetArrayValue() != 0);
        }

        // get nodes with same base number except origin
        var subNodes = Nodes.Where(x =>
            x.Address[0] == address[0]
            && x.Address.GetArrayValue() != address.GetArrayValue());

        for (var i = 1; i < 3; i++)
        {
            if (address.Length <= i || address[i] <= 0) { continue; }

            var i1 = i;
            subNodes = subNodes.Where(x => x.Address.Length > i1 && x.Address[i1] == address[i1]);
        }

        return subNodes;
    }

    public Action<BiDiBNode> NodeRemoved { get; set; }

    public void Reset()
    {
        foreach (var nodeAddress in nodes.Keys)
        {
            RemoveNode(nodeAddress);
        }
    }

    public BiDiBNode GetNodeByShortId(byte[] uniqueId)
    {
        return nodes.Values.FirstOrDefault(x =>
            x.UniqueIdBytes.Skip(2).ToArray().GetArrayValue() == uniqueId.GetArrayValue());
    }

    public byte[] GetNodeAddressByShortId(byte[] uniqueId)
    {
        var node = GetNodeByShortId(uniqueId);

        return node != null ? node.Address : new byte[] { 0 };
    }

    private void RemoveNode(int nodeAddressValue)
    {
        if (nodes.TryRemove(nodeAddressValue, out var nodeToBeRemoved))
        {
            NodeRemoved?.Invoke(nodeToBeRemoved);
        }
        else
        {
            logger.LogWarning($"node with address {nodeAddressValue} could not be removed");
        }
    }
}