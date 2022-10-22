using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models
{
    public class NetBiDiBConnectionStateInfo : ConnectionStateInfo
    {
        public NetBiDiBConnectionStateInfo(
            InterfaceConnectionState interfaceState, 
            NetBiDiBConnectionState localState, 
            NetBiDiBConnectionState remoteState, 
            byte[] remoteId, string remoteName, string remoteAddress, byte[] localAddress,  string error, int timeout) 
            : base(interfaceState, InterfaceConnectionType.NetBiDiB, error)
        {
           
            RemoteId = remoteId;
            RemoteName = remoteName;
            Timeout = timeout;
            LocalState = localState;
            LocalAddress = localAddress;
            RemoteState = remoteState;
            RemoteAddress = remoteAddress;
        }


        public byte[] RemoteId { get; }

        public string RemoteName { get; }

        public string RemoteAddress { get; }

        public int Timeout { get; }

        public byte[] LocalAddress { get; }

        public NetBiDiBConnectionState LocalState { get; }

        public NetBiDiBConnectionState RemoteState { get; }
    }
}