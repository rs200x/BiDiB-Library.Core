namespace org.bidib.Net.Core.Enumerations;

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