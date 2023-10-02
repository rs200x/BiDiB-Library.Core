namespace org.bidib.Net.Core.Enumerations;

public enum RequestResult
{
    GrantedAwaitingResponse =0x00,
    GrantedResponseFollow=0x01,
    GrantedNoResponse = 0x02,
    NodeNotResolved = 0xFD,
    NodeNotAvailable = 0xFE,
    TargetModeNotSupported = 0xFF
}