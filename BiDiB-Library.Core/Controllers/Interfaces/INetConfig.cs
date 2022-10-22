namespace org.bidib.netbidibc.core.Controllers.Interfaces
{
    public interface INetConfig : IConnectionConfig
    {
        string NetworkHostAddress { get; set; }

        int NetworkPortNumber { get; set; }

        string ApplicationName { get; set; }

        string NetBiDiBClientId { get; set; }

        byte NetBiDiBPairingTimeout { get; set; }
    }
}