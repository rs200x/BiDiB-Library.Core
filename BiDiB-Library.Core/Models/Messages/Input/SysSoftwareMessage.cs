namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class SysSoftwareMessage : BiDiBInputMessage
    {
        public SysSoftwareMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_SW_VERSION, 3)
        {
            Version = new[] { MessageParameters[2], MessageParameters[1], MessageParameters[0] };

            //TODO: iterate over parameter tripples to support subversion 
        }

        public byte[] Version { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, V: {Version[0]:d}.{Version[1]:d2}.{Version[2]:d2}";
        }
    }
}