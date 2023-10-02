using System;
using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Models;

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
        IsConnected = InterfaceState is InterfaceConnectionState.FullyConnected or InterfaceConnectionState.PartiallyConnected;
        Error = error;
    }

    public InterfaceConnectionState InterfaceState { get; }

    public InterfaceConnectionType InterfaceType { get; }

    public bool IsConnected { get; }

    public string ConnectionName { get; set; }

    public string Error { get; }
}