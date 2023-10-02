using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Services.EventArgs;

public class ConnectionStateChangedEventArgs : System.EventArgs
{
    public ConnectionStateChangedEventArgs(ConnectionStateInfo stateInfo)
    {
        StateInfo = stateInfo;
    }

    public ConnectionStateInfo StateInfo { get; }
}