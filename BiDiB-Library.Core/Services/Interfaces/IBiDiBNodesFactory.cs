using System;
using System.Collections.Generic;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Services.Interfaces;

public interface IBiDiBNodesFactory
{
    IBiDiBMessageProcessor MessageProcessor { get; set; }

    /// <summary>
    /// Returns collection of current registered nodes
    /// </summary>
    IEnumerable<BiDiBNode> Nodes { get; }

    /// <summary>
    /// Returns the root node of the BiDiB tree
    /// </summary>
    /// <returns>the node instance</returns>
    BiDiBNode GetRootNode();

    /// <summary>
    /// Gets the registered node by its address
    /// </summary>
    /// <param name="address">the address of the node</param>
    /// <returns>the node instance</returns>
    BiDiBNode GetNode(byte[] address);

    /// <summary>
    /// Creates a specific node instance based on unique id information
    /// </summary>
    /// <param name="address">the address of the node</param>
    /// <param name="uniqueId">the unique id of the node</param>
    /// <returns>the specific node instance</returns>
    BiDiBNode CreateNode(byte[] address, byte[] uniqueId);

    /// <summary>
    /// Delegate executed when new node has been added
    /// </summary>
    Action<BiDiBNode> NodeAdded { get; set; }

    /// <summary>
    /// Removes the node from the nodes list
    /// </summary>
    /// <param name="address">the address of the node to be removed</param>
    void RemoveNode(byte[] address);

    /// <summary>
    /// Delegate executed when new node has been removed
    /// </summary>
    Action<BiDiBNode> NodeRemoved { get; set; }

    /// <summary>
    /// Resets the nodes list
    /// </summary>
    void Reset();

    /// <summary>
    /// Determines the node matching the uniqueId
    /// </summary>
    /// <param name="uniqueId">The 5 byte unique id</param>
    /// <returns>The node</returns>
    BiDiBNode GetNodeByShortId(byte[] uniqueId);

    /// <summary>
    /// Determines the address of the node matching the uniqueId
    /// </summary>
    /// <param name="uniqueId">The 5 byte unique id</param>
    /// <returns>The node address</returns>
    byte[] GetNodeAddressByShortId(byte[] uniqueId);
}