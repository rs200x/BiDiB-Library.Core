using System;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class SysMagicMessage : BiDiBInputMessage
    {
        public SysMagicMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_MAGIC, 2)
        {
            MagicLow = MessageParameters[0];
            MagicHigh = MessageParameters[1];
            Magic = BitConverter.ToUInt16(MessageParameters, 0);
        }

        public byte MagicHigh { get; }

        public byte MagicLow { get; }

        public ushort Magic { get; }

        public override string ToString() => $"{base.ToString()}, Low:{MagicLow:X} High:{MagicHigh:X}";
    }
}