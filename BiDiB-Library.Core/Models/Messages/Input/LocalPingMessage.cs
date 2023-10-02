namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_LOCAL_PING)]
public class LocalPingMessage : BiDiBInputMessage
{
    public LocalPingMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_PING, 0)
    {
    }
}
