namespace org.bidib.netbidibc.core.Enumerations
{
    public enum NetBiDiBConnectionState
    {
        Disconnected,
        SendSignature,
        WaitForId,
        RequestPairing,
        WaitForStatus,
        Paired,
        Unpaired,
        PairingRejected,
        ConnectedControlling,
        ConnectedUncontrolled
    }
}