using org.bidib.netbidibc.core.Models;

namespace org.bidib.netbidibc.core.Services.EventArgs
{
    public class ConnectionStateChangedEventArgs : System.EventArgs
    {
        public ConnectionStateChangedEventArgs(ConnectionStateInfo stateInfo)
        {
            StateInfo = stateInfo;
        }

        public ConnectionStateInfo StateInfo { get; }
    }
}