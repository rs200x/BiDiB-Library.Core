// ReSharper disable InconsistentNaming
namespace org.bidib.Net.Core.Enumerations;

public enum FirmwareUpdateOperation
{
    /// <summary>
    /// Node should switch to update mode. 0x00
    /// </summary>
    BIDIB_MSG_FW_UPDATE_OP_ENTER = 0x00,

    /// <summary>
    /// Node shall exit the update mode. 0x01
    /// </summary>
    BIDIB_MSG_FW_UPDATE_OP_EXIT = 0x01,

    /// <summary>
    /// Select target memory. 0x02
    /// </summary>
    BIDIB_MSG_FW_UPDATE_OP_SETDEST = 0x02,

    /// <summary>
    /// A data set will be send for the currently selected target memory. 0x03
    /// </summary>
    BIDIB_MSG_FW_UPDATE_OP_DATA = 0x03,

    /// <summary>
    /// No more data available for the currently selected target memory. 0x04
    /// </summary>
    BIDIB_MSG_FW_UPDATE_OP_DONE = 0x04
}