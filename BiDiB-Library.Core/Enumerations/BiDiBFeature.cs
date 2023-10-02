// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
namespace org.bidib.Net.Core;

public enum BiDiBFeature : byte
{
    //===============================================================================
    //
    // 4. Feature Codes
    //
    //===============================================================================

    //-- occupancy
    FEATURE_BM_SIZE = 0,   // number of occupancy detectors
    FEATURE_BM_ON = 1,   // occupancy detection on/off
    FEATURE_BM_SECACK_AVAILABLE = 2,   // secure ack available
    FEATURE_BM_SECACK_ON = 3,   // secure ack on/off
    FEATURE_BM_CURMEAS_AVAILABLE = 4,   // occupancy detectors with current measurement results
    FEATURE_BM_CURMEAS_INTERVAL = 5,   // interval for current measurements
    FEATURE_BM_DC_MEAS_AVAILABLE = 6,   // (dc) measurements available, even if track voltage is off
    FEATURE_BM_DC_MEAS_ON = 7,   // dc measurement enabled
    FEATURE_BM_ADDR_DETECT_AVAILABLE = 8,   // detector ic capable to detect loco address
    FEATURE_BM_ADDR_DETECT_ON = 9,   // address detection enabled
    FEATURE_BM_ADDR_AND_DIR = 10,   // address detection contains direction
    //-- bidi detection
    FEATURE_BM_ISTSPEED_AVAILABLE = 11,   // speed messages available
    FEATURE_BM_ISTSPEED_INTERVAL = 12,   // speed update interval
    FEATURE_BM_CV_AVAILABLE = 13,   // CV readback available
    FEATURE_BM_CV_ON = 14,   // CV readback enabled
    //-- booster
    FEATURE_BST_VOLT_ADJUSTABLE = 15,   // booster output voltage is adjustable
    FEATURE_BST_VOLT = 16,   // booster output voltage setting (unit: V)
    FEATURE_BST_CUTOUT_AVAIALABLE = 17,   // booster can do cutout for railcom
    FEATURE_BST_CUTOUT_ON = 18,   // cutout is enabled
    FEATURE_BST_TURNOFF_TIME = 19,   // time in ms until booster turns off in case of a short (unit 2ms)
    FEATURE_BST_INRUSH_TURNOFF_TIME = 20,   // time in ms until booster turns off in case of a short after the first power up (unit 2ms)
    FEATURE_BST_AMPERE_ADJUSTABLE = 21,   // booster output current is adjustable
    FEATURE_BST_AMPERE = 22,   // booster output current value (special coding)
    FEATURE_BST_CURMEAS_INTERVAL = 23,   // current update interval
    FEATURE_BST_CV_AVAILABLE = 24,   // (deprecated, now synonym to 13) CV readback available
    FEATURE_BST_CV_ON = 25,   // (deprecated, now synonym to 14) CV readback enabled
    FEATURE_BST_INHIBIT_AUTOSTART = 26,   // 1: Booster does no automatic BOOST_ON when DCC at input wakes up.
    FEATURE_BST_INHIBIT_LOCAL_ONOFF = 27,   // 1: Booster announces local STOP/GO key stroke only, no local action

    //-- bidi detection & occupacy
    FEATURE_BM_DYN_STATE_INTERVAL = 28,   // transmit interval of MSG_BM_DYN_STATE (unit 100ms)
    FEATURE_BM_RCPLUS_AVAILABLE = 29,   // 1: RailcomPlus messages available
    FEATURE_BM_TIMESTAMP_ON = 30,   // 1: OCC will be sent with timestamp
    FEATURE_BM_POSITION_ON = 31,   // position messages enabled
    FEATURE_BM_POSITION_SECACK = 32,   // secure position ack interval (unit: 10ms), 0: none

    //-- accessory
    FEATURE_ACCESSORY_COUNT = 40,   // number of objects
    FEATURE_ACCESSORY_SURVEILLED = 41,   // 1: announce if operated outside bidib
    FEATURE_ACCESSORY_MACROMAPPED = 42,   // 1..n: no of accessory aspects are mapped to macros

    //-- control
    FEATURE_CTRL_INPUT_COUNT = 50,   // number of inputs for keys
    FEATURE_CTRL_INPUT_NOTIFY = 51,   // 1: report a keystroke to host
    FEATURE_CTRL_SWITCH_COUNT = 52,   // number of switch ports (direct controlled)
    FEATURE_CTRL_LIGHT_COUNT = 53,   // number of light ports (direct controlled)
    FEATURE_CTRL_SERVO_COUNT = 54,   // number of servo ports (direct controlled)
    FEATURE_CTRL_SOUND_COUNT = 55,   // number of sound ports (direct controlled)
    FEATURE_CTRL_MOTOR_COUNT = 56,   // number of motor ports (direct controlled)
    FEATURE_CTRL_ANALOGOUT_COUNT = 57,   // number of analog ports (direct controlled)
    FEATURE_CTRL_STRETCH_DIMM = 58,   // additional time stretch for dimming (for light ports)
    FEATURE_CTRL_BACKLIGHT_COUNT = 59,   // number of backlight ports (intensity direct controlled)
    FEATURE_CTRL_MAC_LEVEL = 60,   // supported macro level
    FEATURE_CTRL_MAC_SAVE = 61,   // number of permanent storage places for macros
    FEATURE_CTRL_MAC_COUNT = 62,   // number of macros
    FEATURE_CTRL_MAC_SIZE = 63,   // length of each macro (entries)
    FEATURE_CTRL_MAC_START_MAN = 64,   // (local) manual control of macros enabled
    FEATURE_CTRL_MAC_START_DCC = 65,   // (local) dcc control of macros enabled
    FEATURE_CTRL_PORT_QUERY_AVAILABLE = 66,   // 1: node will answer to MSG_LC_OUTPUT_QUERY
    FEATURE_SWITCH_CONFIG_AVAILABLE = 67,   // (deprecated, version >= 0.6 uses availability of PCFG_IO_CTRL) 1: node has possibility to configure switch ports
    FEATURE_CTRL_PORT_FLAT_MODEL = 70,   // node uses flat port model, number or addressable ports
    FEATURE_CTRL_PORT_FLAT_MODEL_EXTENDED = 71, // node uses flat port model, "high" number of addressable ports

    //-- dcc gen
    FEATURE_GEN_SPYMODE = 100,   // 1: watch bidib handsets
    FEATURE_GEN_WATCHDOG = 101,   // 0: no watchdog, 1: permanent update of MSG_CS_SET_STATE required, unit 100ms
    FEATURE_GEN_DRIVE_ACK = 102,   // not used
    FEATURE_GEN_SWITCH_ACK = 103,   // not used
    FEATURE_GEN_LOK_DB_SIZE = 104,   
    FEATURE_GEN_LOK_DB_STRING = 105,   
    FEATURE_GEN_POM_REPEAT = 106,   // supported service modes
    FEATURE_GEN_DRIVE_BUS = 107,   // 1: this node drive the dcc bus.
    FEATURE_GEN_LOK_LOST_DETECT = 108,   // 1: command station annouces lost loco
    FEATURE_GEN_NOTIFY_DRIVE_MANUAL = 109,   // 1: dcc gen reports manual operation
    FEATURE_GEN_START_STATE = 110,   // 1: power up state, 0=off, 1=on
    FEATURE_GEN_EXT_AVAILABLE = 111,   // 1: supports rcplus messages
    FEATURE_CELL_NUMBER = 112,  // (logical) reference mark of generator (0=single system, 1..n:'area mark')
    FEATURE_RF_CHANNEL = 113,  // used rf channel, 0..83 channel assignment in 2.4 GHz band

    FEATURE_STRING_DEBUG = 251,  // use namespace 1 for debug strings, 0:n.a./silent (default); 1=mode 1
    FEATURE_STRING_SIZE = 252,   // length of user strings, 0:n.a (default); allowed 8..24
    FEATURE_RELEVANT_PID_BITS = 253,   // how many bits of 'vendor32' are relevant for PID (default 16, LSB aligned)
    FEATURE_FW_UPDATE_MODE = 254,   // 0: no fw-update, 1: intel hex (max. 10 byte / record)
    FEATURE_EXTENSION = 255  // 1: reserved for future expansion
}