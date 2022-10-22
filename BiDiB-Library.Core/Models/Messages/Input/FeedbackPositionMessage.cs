using System;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class FeedbackPositionMessage : BiDiBInputMessage
    {
        public FeedbackPositionMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_POSITION, 5)
        {
            AddressLow = MessageParameters[0];
            AddressHigh = MessageParameters[1];
            FeedbackAddress = BitConverter.ToUInt16(new[] { AddressLow, AddressHigh }, 0);

            FeedbackType = MessageParameters[2];

            LocationLow = MessageParameters[3];
            LocationHigh = MessageParameters[4];
            Location = BitConverter.ToUInt16(new[] { LocationLow, LocationHigh }, 0);
        }

        public ushort Location { get; set; }

        public byte LocationHigh { get; set; }

        public byte LocationLow { get; set; }

        public byte FeedbackType { get; set; }

        public ushort FeedbackAddress { get; set; }

        public byte AddressHigh { get; set; }

        public byte AddressLow { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, FAddr: {FeedbackAddress}, FLoc: {Location}, FType: {FeedbackType}";
        }
    }
}