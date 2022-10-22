// ReSharper disable InconsistentNaming
namespace org.bidib.netbidibc.core.Enumerations
{
    public enum BoosterState : byte
    {
        BIDIB_BST_STATE_OFF = 0x00,
        BIDIB_BST_STATE_OFF_SHORT = 0x01,
        BIDIB_BST_STATE_OFF_HOT = 0x02,
        BIDIB_BST_STATE_OFF_NOPOWER = 0x03,
        BIDIB_BST_STATE_OFF_GO_REQ = 0x04,
        BIDIB_BST_STATE_OFF_HERE = 0x05,
        BIDIB_BST_STATE_OFF_NO_DCC = 0x06,
        BIDIB_BST_STATE_ON = 0x80,
        BIDIB_BST_STATE_ON_LIMIT = 0x81,
        BIDIB_BST_STATE_ON_HOT = 0x82,
        BIDIB_BST_STATE_ON_STOP_REQ = 0x83,
        BIDIB_BST_STATE_ON_HERE = 0x84
    }
}