namespace org.bidib.netbidibc.core.Enumerations
{
    public enum AccessoryState : byte
    {
        BIDIB_ACC_STATE_DONE = 0x00,  // done
        BIDIB_ACC_STATE_WAIT = 0x01,   // not done, time (like railcom spec) following
        BIDIB_ACC_STATE_NO_FB_AVAILABLE = 0x02,  // ...and no feedback available
        BIDIB_ACC_STATE_ERROR = 0x80,  // error, error code following
    }
}