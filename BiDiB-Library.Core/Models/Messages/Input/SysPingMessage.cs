namespace org.bidib.netbidibc.core.Models.Messages.Input;

public class SysPingMessage : BiDiBInputMessage
{
    public SysPingMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_PING, 0)
    {
    }
}
