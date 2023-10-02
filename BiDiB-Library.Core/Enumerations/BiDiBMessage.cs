// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
namespace org.bidib.Net.Core;

public enum BiDiBMessage : byte
{
    Undefined = 0,
    //===============================================================================
    //
    // 1. Defines for Downstream Messages
    //
    //===============================================================================
    //*// = broadcast messages, a interface must forward this to subnodes
    //      a node must not answer these messages, if not connected

    //-- system messages                                    // Parameters
    MSG_SYS_GET_MAGIC = 1,       // - // these must stay here
    MSG_SYS_GET_P_VERSION = 2,       // - // these must stay here
    MSG_SYS_ENABLE = 3,    //*// -
    MSG_SYS_DISABLE = 4,    //*// -
    MSG_SYS_GET_UNIQUE_ID = 5,       // -
    MSG_SYS_GET_SW_VERSION = 6,       // -
    MSG_SYS_PING = 7,       // 1:dat
    MSG_SYS_IDENTIFY = 8,       // 1:id_state
    MSG_SYS_RESET = 9,    //*// -
    MSG_GET_PKT_CAPACITY = 10,       // -
    MSG_NODETAB_GETALL = 11,       // -
    MSG_NODETAB_GETNEXT = 12,       // -
    MSG_NODE_CHANGED_ACK = 13,       // 1:nodetab_version
    MSG_SYS_GET_ERROR = 14,       // -
    MSG_FW_UPDATE_OP = 15,       // 1:opcode, 2..n parameters

    //-- feature and user config messages
    MSG_FEATURE_GETALL = 16,        // -
    MSG_FEATURE_GETNEXT = 17,        // -
    MSG_FEATURE_GET = 18,        // 1:feature_num
    MSG_FEATURE_SET = 19,        // 1:feature_num, 2:feature_val
    MSG_VENDOR_ENABLE = 20,        // 1-7: unique-id of node
    MSG_VENDOR_DISABLE = 21,        // -
    MSG_VENDOR_SET = 22,        // V_NAME,V_VALUE
    MSG_VENDOR_GET = 23,        // V_NAME
    MSG_SYS_CLOCK = 24,     //*// 1:TCODE0, 2:TCODE1, 3:TCODE2, 4:TCODE3
    MSG_STRING_GET = 25,        // 1:Nspace, 2:ID
    MSG_STRING_SET = 26,        // 1:Nspace, 2:ID, 3:Strsize, 4...n: string

    //-- occupancy messages
    MSG_BM_GET_RANGE = 32,        // 1:start, 2:end
    MSG_BM_MIRROR_MULTIPLE = 33,        // 1:start, 2:size, 3..n:data
    MSG_BM_MIRROR_OCC = 34,        // 1:mnum
    MSG_BM_MIRROR_FREE = 35,        // 1:mnum
    MSG_BM_ADDR_GET_RANGE = 36,        // 1:start, 2:end
    MSG_BM_GET_CONFIDENCE = 37,        // -
    MSG_BM_MIRROR_POSITION = 38,        // experimental

    //-- booster messages
    MSG_BOOST_OFF = 48,    //*// 1:bc or node
    MSG_BOOST_ON = 49,    //*// 1:bc or node
    MSG_BOOST_QUERY = 50,       // -

    //-- accessory control messages
    MSG_ACCESSORY_SET = 56,       // 1:anum, 2:aspect
    MSG_ACCESSORY_GET = 57,       // 1:anum
    MSG_ACCESSORY_PARA_SET = 58,       // 1:anum, 2:para_num, 3..n: data
    MSG_ACCESSORY_PARA_GET = 59,       // 1:anum, 2:para_num

    //-- switch/light/servo control messages
    MSG_LC_PORT_QUERY_ALL = 63,     // 1:selL, 2:selH, [3:startL, 4:startH, 5:endL, 6:endH]
    MSG_LC_OUTPUT = 64,        // 1:type, 2:port, 3:state
    MSG_LC_CONFIG_SET = 65,        // 1:type, 2:port, 3:off_val, 4:on_val, 5:dimm_off, 6:dimm_on
    MSG_LC_CONFIG_GET = 66,        // 1:type, 2:port
    MSG_LC_KEY_QUERY = 67,        // 1:port
    MSG_LC_PORT_QUERY = 68,     // 1:type, 2:port
    MSG_LC_CONFIGX_GET_ALL = 69,       // [1:startL, 2:startH, 3:endL, 4:endH]
    MSG_LC_CONFIGX_SET = 70,        // 1:type, 2:port, [3:p_enum, 4:p_val]  (up to 16)
    MSG_LC_CONFIGX_GET = 71,       // 1:type, 2:port

    //-- macro messages
    MSG_LC_MACRO_HANDLE = 72,       // 1:macro, 2:opcode
    MSG_LC_MACRO_SET = 73,       // 1:macro, 2:item, 3:delay, 4:lstate,  5:lvalue
    MSG_LC_MACRO_GET = 74,       // 1:macro, 2:item
    MSG_LC_MACRO_PARA_SET = 75,       // 1:macro, 2:para_idx, 3,4,5,6:value
    MSG_LC_MACRO_PARA_GET = 76,       // 1:macro, 2:para_idx

    //-- distributed control messages
    MSG_GUEST_RESP_SUBSCRIPTION_COUNT = 80,
    MSG_GUEST_RESP_SUBSCRIPTION = 81,
    MSG_GUEST_RESP_SENT = 82,
    MSG_GUEST_RESP_NOTIFY = 83,

    //-- dcc gen messages
    MSG_CS_ALLOCATE = 96,
    MSG_CS_SET_STATE = 98,       // 1:state
    MSG_CS_DRIVE = 100,       // 1:addrl, 2:addrh, 3:format, 4:active, 5:speed, 6:1-4, 7:5-12, 8:13-20, 9:21-28
    MSG_CS_ACCESSORY = 101,       // 1:addrl, 2:addrh, 3:data(aspect), 4:time_l, 5:time_h
    MSG_CS_BIN_STATE = 102,       // 1:addrl, 2:addrh, 3:bin_statl, 4:bin_stath
    MSG_CS_POM = 103,       // 1..4:addr/did, 5:MID, 6:opcode, 7:cv_l, 8:cv_h, 9:cv_x, 10..13: data
    MSG_CS_RCPLUS = 104,       // 1:opcode, [2..n:parameter]
    MSG_CS_PROG = 111,       // 1:opcode, 2:cv_l, 3:cv_h, 4: data

    //-- local message    // only locally used
    MSG_LOCAL_LOGON_ACK = 112,     // 1:node_addr, 2..8:unique_id
    MSG_LOCAL_PING = 113,
    MSG_LOCAL_LOGON_REJECTED = 114,     // 1..7:unique_id
    MSG_LOCAL_ACCESSORY = 115,  //*// 1:status flag, 2,3: DCC-accessory addr
    MSG_LOCAL_SYNC = 116,     // 1:time_l, 2:time_h

    MSG_LOCAL_EMITTER = 127,

    //===============================================================================
    //
    // 2. Defines for Upstream Messages
    //
    //===============================================================================

    //-- system messages
    MSG_SYS_MAGIC = 129,       // 1:0xFE 2:0xAF
    MSG_SYS_PONG = 130,       // 1:mirrored dat
    MSG_SYS_P_VERSION = 131,       // 1:proto-ver_l, 2:proto-ver_h
    MSG_SYS_UNIQUE_ID = 132,       // 1:class, 2:classx, 3:vid, 4..7:pid+uid
    MSG_SYS_SW_VERSION = 133,       // 1:sw-ver_l, 2:sw_-ver_h, 3:sw-ver_u
    MSG_SYS_ERROR = 134,       // 1:err_code, 2:msg
    MSG_SYS_IDENTIFY_STATE = 135,       // 1:state
    MSG_NODETAB_COUNT = 136,       // 1:length
    MSG_NODETAB = 137,       // 1:version, 2:local num, 3..9: unique
    MSG_PKT_CAPACITY = 138,       // 1:capacity
    MSG_NODE_NA = 139,       // 1:node
    MSG_NODE_LOST = 140,       // 1:node
    MSG_NODE_NEW = 141,       // 1:version, 2:local num, 3..9: unique
    MSG_STALL = 142,       // 1:state
    MSG_FW_UPDATE_STAT = 143,       // 1:stat, 2:timeout

    //-- feature and user config messages
    //MSG_UFC = (MSG_USTRM + 0x10),
    MSG_FEATURE = 144,        // 1:feature_num, 2:data
    MSG_FEATURE_NA = 145,        // 1:feature_num
    MSG_FEATURE_COUNT = 146,        // 1:count
    MSG_VENDOR = 147,        // 1..n: length,'string',length,'value'
    MSG_VENDOR_ACK = 148,        // 1:ack
    MSG_STRING = 149,        // 1:namespace, 2:id, 3:stringsize, 4...n: string

    //-- occupancy and bidi-detection messages
    //MSG_UBM = (MSG_USTRM + 0x20),
    MSG_BM_OCC = 160,        // 1:mnum
    MSG_BM_FREE = 161,        // 1:mnum [2,3:time_l, time_h]
    MSG_BM_MULTIPLE = 162,        // 1:base, 2:size; 3..n:data
    MSG_BM_ADDRESS = 163,        // 1:mnum, [2,3:addr_l, addr_h]
    MSG_BM_ACCESSORY = 164,        // (reserved, do not use yet), 1:mnum, [2,3:addr_l, addr_h]
    MSG_BM_CV = 165,        // 1:addr_l, 2:addr_h, 3:cv_addr_l, 4:cv_addr_h, 5:cv_dat
    MSG_BM_SPEED = 166,        // 1:addr_l, 2:addr_h, 3:speed_l, 4:speed_h (from loco),
    MSG_BM_CURRENT = 167,        // 1:mnum, 2:current
    MSG_BM_XPOM = 168,        // 1:decvid, 2..5:decuid, 6:offset, 7:idxl, 8:idxh, 9..12:data
    MSG_BM_CONFIDENCE = 169,        // 1:void, 2:freeze, 3:signal
    MSG_BM_DYN_STATE = 170,        // 1:mnum, 2:addr_l, 3:addr_h, 4:dyn_num, 5:value (from loco),
    MSG_BM_RCPLUS = 171,        // 1:mnum, 2:opcode, [3..n:parameter]
    MSG_BM_POSITION = 172,       // 1:type, 2,3:addr_l, addr_h, 4,5:locid_l, locid_h, 6 locid_type (experimental)

    //-- booster messages
    MSG_BOOST_STAT = 176,       // 1:state (see defines below),
    //MSG_BOOST_CURRENT = 177,       // (deprecated by DIAGNOSTIC with V0.10), 1:current
    MSG_BOOST_DIAGNOSTIC = 178,       // [1:enum, 2:value],[3:enum, 4:value] ...
    // MSG_NEW_DECODER = 179,       // was reserved for MSG_NEW_DECODER (deprecated), 1:mnum, 2: dec_vid, 3..6:dec_uid
    //MSG_ID_SEARCH_ACK = 180,       // was reserved for MSG_ID_SEARCH_ACK (deprecated), 1:mnum, 2: s_vid, 3..6:s_uid[0..3], 7: dec_vid, 8..11:dec_uid
    //MSG_ADDR_CHANGE_ACK = 181,       // was reserved for MSG_ADDR_CHANGE_ACK (deprecated), 1:mnum, 2: dec_vid, 3..6:dec_uid, 7:addr_l, 8:addr_h

    //-- accessory control messages
    MSG_ACCESSORY_STATE = 184,       // 1:port, 2:aspect, 3:total, 4:execute, 5:wait (Quittung),
    MSG_ACCESSORY_PARA = 185,       // 1:anum, 2:para_num, 3..n: data
    MSG_ACCESSORY_NOTIFY = 186,       // 1:port, 2:aspect, 3:total, 4:execute, 5:wait (Spontan),

    //-- switch/light control messages
    MSG_LC_STAT = 192,        // 1:type, 2:port, 3:state
    MSG_LC_NA = 193,        // 1:type, 2:port, [3:errcause]
    MSG_LC_CONFIG = 194,        // 1:type, 2:port, 3:off_val, 4:on_val, 5:dimm_off, 6:dimm_on
    MSG_LC_KEY = 195,        // 1:port, 2:state
    MSG_LC_WAIT = 196,        // 1:type, 2:port, 3:time
    MSG_LC_CONFIGX = 198,        // 1:type, 2:port, [3:p_enum, 4:p_val]  (up to 16),

    //-- macro messages
    MSG_LC_MACRO_STATE = 200,
    MSG_LC_MACRO = 201,
    MSG_LC_MACRO_PARA = 202,

    //-- distributed control messages
    MSG_GUEST_REQ_SUBSCRIBE = 208,
    MSG_GUEST_REQ_UNSUBSCRIBE = 209,
    MSG_GUEST_REQ_SEND = 210,

    //-- dcc control messages
    MSG_CS_ALLOC_ACK = 224,       // noch genauer zu klaeren / to be specified
    MSG_CS_STATE = 225,
    MSG_CS_DRIVE_ACK = 226,
    MSG_CS_ACCESSORY_ACK = 227,       // 1:addrl, 2:addrh, 3:data
    MSG_CS_POM_ACK = 228,       // 1:addrl, 2:addrh, 3:addrxl, 4:addrxh, 5:mid, 6:data
    MSG_CS_DRIVE_MANUAL = 229,       // 1:addrl, 2:addrh, 3:format, 4:active, 5:speed, 6:1-4, 7:5-12, 8:13-20, 9:21-28
    MSG_CS_DRIVE_EVENT = 230,       // 1:addrl, 2:addrh, 3:eventtype, Parameters
    MSG_CS_ACCESSORY_MANUAL = 231,       // 1:addrl, 2:addrh, 3:data
    MSG_CS_RCPLUS_ACK = 232,       // 1:opcode, [2..n:parameter]
    MSG_CS_PROG_STATE = 239,       // 1: state, 2:time, 3:cv_l, 4:cv_h, 5:data

    //-- local message     // only locally used
    MSG_LOCAL_LOGON = 240,
    MSG_LOCAL_PONG = 241,    // only locally used
    MSG_LOCAL_LOGOFF = 242,
    MSG_LOCAL_ANNOUNCE = 243,
    MSG_LOCAL_PROTOCOL_SIGNATURE = 254,
    MSG_LOCAL_LINK = 255
}