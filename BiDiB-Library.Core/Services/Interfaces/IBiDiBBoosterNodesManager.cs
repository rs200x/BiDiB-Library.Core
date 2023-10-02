using System;
using System.Collections.Generic;
using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Services.Interfaces;

public interface IBiDiBBoosterNodesManager
{

    IEnumerable<BiDiBBoosterNode> Boosters { get; }
    Action<BiDiBBoosterNode> BoosterAdded { get; set; }
    Action<BiDiBBoosterNode> BoosterRemoved { get; set; }

    void NodeAdded(BiDiBNode newNode);

    void NodeRemoved(BiDiBNode removedNode);
       
}