// ReSharper disable InconsistentNaming
namespace org.bidib.netbidibc.core.Enumerations
{
    public enum FirmwareUpdateError
    {
        BIDIB_FW_UPDATE_ERROR_NO_DEST = 0x01,
        BIDIB_FW_UPDATE_ERROR_RECORD = 0x02,
        BIDIB_FW_UPDATE_ERROR_ADDR = 0x03,
        BIDIB_FW_UPDATE_ERROR_CHECKSUM = 0x04,
        BIDIB_FW_UPDATE_ERROR_SIZE = 0x05,
        BIDIB_FW_UPDATE_ERROR_APPCRC = 0x06
    }
}