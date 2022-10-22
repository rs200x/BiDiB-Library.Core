using System;
using System.Collections.Generic;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;

namespace org.bidib.netbidibc.core.Services.Interfaces
{
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
    }
}