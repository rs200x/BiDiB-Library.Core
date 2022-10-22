namespace org.bidib.netbidibc.core.Models.Messages.Input;

public class LocalPingMessage : BiDiBInputMessage
{
    public LocalPingMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_PING, 0)
    {
    }
}
