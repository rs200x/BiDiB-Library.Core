namespace org.bidib.Net.Core;

public static class BiDiBConstants
{
    public static string InMessagePrefix => "IN  <--:";
    public static string OutMessagePrefix => "OUT -->:";

    public const string LoggerContextRaw = "RAW";
    public const string LoggerContextLatency = "LAT";
    public const string LoggerContextMessage = "MS";
    public const string LoggerContextException = "EXC";
}