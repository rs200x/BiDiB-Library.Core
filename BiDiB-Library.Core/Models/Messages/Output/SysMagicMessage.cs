using System;

namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class SysMagicMessage : BiDiBOutputMessage
    {
        public SysMagicMessage(byte[] address) : base(address, BiDiBMessage.MSG_SYS_MAGIC)
        {
            MagicLow = 0xDE;
            MagicHigh = 0xC0;

            Parameters = new[] { MagicLow, MagicHigh};
            Magic = BitConverter.ToUInt16(Parameters, 0);
        }

        public byte MagicHigh { get; }

        public byte MagicLow { get; }

        public ushort Magic { get; }

        public override string ToString() => $"{base.ToString()}, Low:{MagicLow:X} High:{MagicHigh:X}";
    }
}
