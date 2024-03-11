using System;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_BM_DYN_STATE)]
public class FeedbackDynStateMessage : FeedbackMessage
{
    public FeedbackDynStateMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_DYN_STATE, 5)
    {
        DecoderAddress = ByteUtils.GetDecoderAddress(MessageParameters[1], MessageParameters[2]);
        Direction = MessageParameters[2].GetDecoderDirection();
        DynState = (DynState)MessageParameters[3];
        Value = MessageParameters[4];
        Timestamp = (ushort)(MessageParameters.Length >= 8 ? BitConverter.ToUInt16(MessageParameters, 6) : 0);
    }

    public int Value { get; }

    public DynState DynState { get; }

    public ushort DecoderAddress { get; }

    public DecoderDirection Direction { get; }

    public ushort Distance => (ushort)(MessageParameters.Length >= 6 ? BitConverter.ToUInt16(MessageParameters, 4) : 0);

    /// <summary>
    /// Returns the state specific value
    /// </summary>
    /// <returns></returns>
    public int StateValue
    {
        get
        {
            switch (DynState)
            {
                case DynState.SignalQuality:
                case DynState.Container1:
                case DynState.Container2:
                case DynState.Container3:
                case DynState.TrackVoltage:
                    return Value;
                case DynState.Temperature:
                case DynState.Temperature2:
                {
                    return GetTempValue();
                }
                case DynState.Distance:
                {
                    return Distance;
                }
                default:
                    return Value;
            }
        }
    }

    public override string ToString()
    {
        var dynValue = DynState != DynState.Distance ? $"Value: {Value}" : $"Distance: {Distance}";
        return $"{base.ToString()}, Addr: {DecoderAddress}/{Direction}, DynNum: {DynState}, {dynValue}";
    }

    private int GetTempValue()
    {
        return Value switch
        {
            <= 127 => Value,// (stateValue >= 0)  always true
            >= 226 and <= 255 => Value,
            _ => 0
        };
    }
}