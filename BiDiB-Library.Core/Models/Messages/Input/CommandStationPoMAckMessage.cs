using System;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class CommandStationPoMAckMessage : BiDiBInputMessage
    {
        public CommandStationPoMAckMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_CS_POM_ACK, 6)
        {
            AddressLow = MessageParameters[0];
            AddressHigh = Convert.ToByte(MessageParameters[1] & 0x3f);
            AddressXLow = MessageParameters[2];
            AddressXHigh = MessageParameters[3];
            ManufacturerId = MessageParameters[4];
            Receipt = MessageParameters[5];
            DecoderId = new[] { ManufacturerId, AddressXHigh, AddressXLow, AddressHigh, AddressLow };
        }

        public byte[] DecoderId { get; }

        public byte AddressLow { get; }

        public byte AddressXLow { get; }

        public byte AddressHigh { get; }

        public byte AddressXHigh { get; }

        public byte Receipt { get; }

        public byte ManufacturerId { get; }

        public int DecoderAddress => AddressHigh * 256 + AddressLow;

        public override string ToString()
        {
            return $"{base.ToString()} Dec:{DecoderAddress} Data:{Receipt}";
        }
    }
}