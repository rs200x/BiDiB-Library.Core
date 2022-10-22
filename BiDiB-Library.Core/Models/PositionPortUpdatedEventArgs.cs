using System;
using org.bidib.netbidibc.core.Models.BiDiB;

namespace org.bidib.netbidibc.core.Models
{
    public class PositionPortUpdatedEventArgs : EventArgs
    {
        public PositionPortUpdatedEventArgs(PositionPort port)
        {
            Port = port;
        }

        public PositionPort Port { get; }
    }
}