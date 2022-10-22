using System;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class FeedbackCvMessage : BiDiBInputMessage
    {
        public FeedbackCvMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_CV, 5)
        {
            AddressLow = MessageParameters[0];
            AddressHigh = Convert.ToByte(MessageParameters[1] & 0x3f);
            DecoderAddress = BitConverter.ToUInt16(new[] { AddressLow, AddressHigh }, 0);
            CvLow = MessageParameters[2];
            CvHigh = MessageParameters[3];
            Data = MessageParameters[4];
        }

        public ushort DecoderAddress { get; }

        public byte AddressLow { get; }
        public byte AddressHigh { get; }

        public byte CvLow { get; }
        public byte CvHigh { get; }

        public int CvNumber => CvHigh * 256 + CvLow + 1;

        public byte Data { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, Addr: {DecoderAddress}, CV: {CvNumber}, Value: {Data}";
        }
    }
}