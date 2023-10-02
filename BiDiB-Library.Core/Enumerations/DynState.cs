namespace org.bidib.Net.Core.Enumerations;
//*** Status of the third byte of Message MSG_BM_DYN_STATE

public enum DynState 
{
    Reserved = 0,
    SignalQuality = 1,
    Temperature = 2,
    Temperature2 = 26,
    Container1 = 3,
    Container2 = 4,
    Container3 = 5,
    Distance = 6,
    TrackVoltage = 46,
}