// ReSharper disable InconsistentNaming
namespace org.bidib.netbidibc.core.Enumerations
{
    public enum CommandStationProgState : byte
    {
        BIDIB_CS_PROG_START = 0x00,
        BIDIB_CS_PROG_RUNNING = 0x01,
        BIDIB_CS_PROG_OKAY = 0x80,
        BIDIB_CS_PROG_STOPPED = 0xc0,
        BIDIB_CS_PROG_NO_LOCO = 0xc1,
        BIDIB_CS_PROG_NO_ANSWER = 0xc2,
        BIDIB_CS_PROG_SHORT = 0xc3,
        BIDIB_CS_PROG_VERIFY_FAILED = 0xc4
    }

}