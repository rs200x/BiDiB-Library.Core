// ReSharper disable InconsistentNaming
namespace org.bidib.Net.Core.Enumerations;

public enum AccessoryParameter : byte
{
    ACCESSORY_PARA_USES_STRINGS = 249,
    ACCESSORY_PARA_HAS_ESTOP = 250,
    ACCESSORY_PARA_OPMODE = 251,
    ACCESSORY_PARA_STARTUP = 252,   // following data defines initialisation behavior
    ACCESSORY_PARA_MACROMAP = 253,  // following data defines a mapping
    ACCESSORY_SWITCH_TIME = 254,

    /// <summary>
    /// Requested Parameter not exists
    /// </summary>
    ACCESSORY_PARA_NOTEXIST = 255
}