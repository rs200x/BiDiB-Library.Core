using System;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
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

        public int Value { get; set; }

        public DynState DynState { get; set; }

        public ushort DecoderAddress { get; set; }

        public DecoderDirection Direction { get; }

        public short Distance => (short)(MessageParameters.Length >= 6 ? BitConverter.ToInt16(MessageParameters, 4) : 0);

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
                        {
                            return GetTempValue();
                        }
                    case DynState.Distance:
                        {
                            return BitConverter.ToUInt16(MessageParameters, 4);
                        }
                    default:
                        return Value;
                }
            }
        }

        public override string ToString()
        {
            var dynValue = DynState != DynState.Distance ? $"Value: {Value}" : $"Distance: {Distance}, Timestamp: {Timestamp}";
            return $"{base.ToString()}, Addr: {DecoderAddress}, DynNum: {DynState}, {dynValue}, Direction: {Direction}";
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
}