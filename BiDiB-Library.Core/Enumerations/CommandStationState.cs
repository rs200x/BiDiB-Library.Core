// ReSharper disable InconsistentNaming
namespace org.bidib.netbidibc.core.Enumerations
{
    public enum CommandStationState : byte
    {
        BIDIB_CS_STATE_OFF = 0x00,
        BIDIB_CS_STATE_STOP = 0x01,
        BIDIB_CS_STATE_SOFTSTOP = 0x02,
        BIDIB_CS_STATE_GO = 0x03,
        BIDIB_CS_STATE_GO_IGN_WD = 0x04,
        BIDIB_CS_STATE_PROG = 0x08,
        BIDIB_CS_STATE_PROGBUSY = 0x09,
        BIDIB_CS_STATE_BUSY = 0x0d,
        BIDIB_CS_STATE_QUERY = 0xff
    }
}