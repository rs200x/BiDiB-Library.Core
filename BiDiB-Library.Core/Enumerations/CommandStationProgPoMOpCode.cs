// ReSharper disable InconsistentNaming
namespace org.bidib.Net.Core.Enumerations;

/// <summary>
/// MSG_CS_POM Operation Codes
/// </summary>
public enum CommandStationProgPoMOpCode : byte
{
    BIDIB_CS_POM_RD_BLOCK = 0x00,
    BIDIB_CS_POM_RD_BYTE = 0x01,
    BIDIB_CS_POM_WR_BIT = 0x02,
    BIDIB_CS_POM_WR_BYTE = 0x03,
    BIDIB_CS_xPOM_RD_BLOCK = 0x81,
    BIDIB_CS_xPOM_WR_BIT = 0x82,
    BIDIB_CS_xPOM_WR_BYTE1 = 0x83,
    BIDIB_CS_xPOM_WR_BYTE2 = 0x84,
    BIDIB_CS_xPOM_WR_BYTE3 = 0x8b,
    BIDIB_CS_xPOM_WR_BYTE4 = 0x8f
}