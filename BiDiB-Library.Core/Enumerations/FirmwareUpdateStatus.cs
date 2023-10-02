// ReSharper disable InconsistentNaming
namespace org.bidib.Net.Core.Enumerations;

public enum FirmwareUpdateStatus : byte
{
    BIDIB_MSG_FW_UPDATE_STAT_READY = 0x00,
    BIDIB_MSG_FW_UPDATE_STAT_EXIT = 0x01,
    BIDIB_MSG_FW_UPDATE_STAT_DATA = 0x02,
    BIDIB_MSG_FW_UPDATE_STAT_ERROR = 0xff
}