using System;
using org.bidib.Net.Core.Models.BiDiB;

namespace org.bidib.Net.Core.Models;

public class PositionPortUpdatedEventArgs : EventArgs
{
    public PositionPortUpdatedEventArgs(PositionPort port)
    {
        Port = port;
    }

    public PositionPort Port { get; }
}