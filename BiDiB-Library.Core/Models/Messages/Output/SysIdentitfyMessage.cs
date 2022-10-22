namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class SysIdentifyMessage : BiDiBOutputMessage
    {
        public SysIdentifyMessage(byte[] address, bool enabled) : base(address, BiDiBMessage.MSG_SYS_IDENTIFY)
        {
            Enabled = enabled;
            Parameters = new[] { (byte)(enabled ? 0x01 : 0x00) };
        }

        public bool Enabled { get; }

        public override string ToString() => $"{base.ToString()}, enabled:{Enabled}";
    }
}