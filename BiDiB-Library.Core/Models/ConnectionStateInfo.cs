using System;
using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models
{
    [Serializable]
    public class ConnectionStateInfo
    {
        public ConnectionStateInfo(InterfaceConnectionState interfaceState, InterfaceConnectionType interfaceType) : this(interfaceState, interfaceType, null)
        {
        }

        public ConnectionStateInfo(InterfaceConnectionState interfaceState, InterfaceConnectionType interfaceType, string error)
        {
            InterfaceState = interfaceState;
            InterfaceType = interfaceType;
            IsConnected = InterfaceState == InterfaceConnectionState.FullyConnected || InterfaceState == InterfaceConnectionState.PartiallyConnected;
            Error = error;
        }

        public InterfaceConnectionState InterfaceState { get; }

        public InterfaceConnectionType InterfaceType { get; }

        public bool IsConnected { get; }

        public string ConnectionName { get; set; }

        public string Error { get; }
    }
}