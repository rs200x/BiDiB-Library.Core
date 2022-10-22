// ReSharper disable InconsistentNaming
namespace org.bidib.netbidibc.core.Enumerations
{
    public enum CommandStationProgTrackOpCode : byte
    {
        BIDIB_CS_PROG_BREAK = 0x00,
        BIDIB_CS_PROG_QUERY = 0x01,
        BIDIB_CS_PROG_RD_BYTE = 0x02,
        BIDIB_CS_PROG_RDWR_BIT = 0x03,
        BIDIB_CS_PROG_WR_BYTE = 0x04
    }

}