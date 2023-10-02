namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_PING)]
public class SysPingMessage : BiDiBInputMessage
{
    public SysPingMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_PING, 0)
    {
    }
}
