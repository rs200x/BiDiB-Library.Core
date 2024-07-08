// ReSharper disable InconsistentNaming
namespace org.bidib.Net.Core.Enumerations;

public enum LocalLinkType : byte
{
    DESCRIPTOR_UID = 0xFF,
    DESCRIPTOR_PROD_STRING = 0x00,
    DESCRIPTOR_USER_STRING = 0x01,
    DESCRIPTOR_ROLE = 0x7F,
    DESCRIPTOR_P_VERSION = 0x80,
    NODE_UNAVAILABLE = 0x81, // 2:size, 3..m:chars, m+1: size, m+2..n:chars
    NODE_AVAILABLE = 0x82, 
    PAIRING_REQUEST = 0xFC,
    STATUS_UNPAIRED = 0xFD,
    STATUS_PAIRED = 0xFE
}