namespace org.bidib.Net.Core.Enumerations;

public enum AccessoryErrorState
{
    BIDIB_ACC_STATE_ERROR_MORE = 0x40,  // more errors are present
    BIDIB_ACC_STATE_ERROR_NONE = 0x00,   // no (more) errors
    BIDIB_ACC_STATE_ERROR_VOID = 0x01,  // no processing possible, illegal aspect
    BIDIB_ACC_STATE_ERROR_CURRENT = 0x02,   // current comsumption to high
    BIDIB_ACC_STATE_ERROR_LOWPOWER = 0x03,  // supply too low
    BIDIB_ACC_STATE_ERROR_FUSE = 0x04,   // fuse blown
    BIDIB_ACC_STATE_ERROR_TEMP = 0x05,   // temp too high
    BIDIB_ACC_STATE_ERROR_POSITION = 0x06,   // feedback error
    BIDIB_ACC_STATE_ERROR_MAN_OP = 0x07,   // manually operated
    BIDIB_ACC_STATE_ERROR_BULB = 0x10,   // bulb blown
    BIDIB_ACC_STATE_ERROR_SERVO = 0x20,   // servo broken
    BIDIB_ACC_STATE_ERROR_SELFTEST = 0x3F,   // internal error
}