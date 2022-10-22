using System;
using System.Collections.Generic;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;

namespace org.bidib.netbidibc.core.Services.Interfaces
{
    internal interface IBiDiBBoosterNodesManager : IMessageReceiver
    {

        IEnumerable<BiDiBBoosterNode> Boosters { get; }
        Action<BiDiBBoosterNode> BoosterAdded { get; set; }
        Action<BiDiBBoosterNode> BoosterRemoved { get; set; }

        void NodeAdded(BiDiBNode newNode);

        void NodeRemoved(BiDiBNode removedNode);
       
    }
}