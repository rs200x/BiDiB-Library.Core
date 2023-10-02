// ReSharper disable InconsistentNaming
namespace org.bidib.Net.Core.Enumerations;

public enum SystemError : byte
{
    BIDIB_ERR_NONE = 0x00,
    BIDIB_ERR_TXT = 0x01,
    BIDIB_ERR_CRC = 0x02,
    BIDIB_ERR_SIZE = 0x03,
    BIDIB_ERR_SEQUENCE = 0x04,
    BIDIB_ERR_PARAMETER = 0x05,
    BIDIB_ERR_BUS = 0x10,
    BIDIB_ERR_ADDRSTACK = 0x11,
    BIDIB_ERR_IDDOUBLE = 0x12,
    BIDIB_ERR_SUBCRC = 0x13,
    BIDIB_ERR_SUBTIME = 0x14,
    BIDIB_ERR_SUBPAKET = 0x15,
    BIDIB_ERR_OVERRUN = 0x16,
    BIDIB_ERR_HW = 0x20,
    BIDIB_ERR_RESET_REQUIRED = 0x21,
    BIDIB_ERR_NO_ACK_BY_HOST = 0x30  // BM (OCC or FREE) was not mirrored by host
}