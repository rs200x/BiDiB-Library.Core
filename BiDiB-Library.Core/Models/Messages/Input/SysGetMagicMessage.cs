namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class SysGetMagicMessage : BiDiBInputMessage
    {
        public SysGetMagicMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_GET_MAGIC, 0)
        {
        }
    }
}