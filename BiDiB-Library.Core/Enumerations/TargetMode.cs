﻿namespace org.bidib.Net.Core.Enumerations;

public enum TargetMode : byte
{
    BIDIB_TARGET_MODE_UID = 0x00,

    BIDIB_TARGET_MODE_ALL = 0x01,

    BIDIB_TARGET_MODE_TOP = 0x02,

    BIDIB_TARGET_MODE_SWITCH = 0x08,

    BIDIB_TARGET_MODE_BOOSTER = 0x09,

    BIDIB_TARGET_MODE_ACCESSORY = 0x0A,

    BIDIB_TARGET_MODE_DCCGEN = 0x0C,

    BIDIB_TARGET_MODE_HUB = 0x0F,
}
