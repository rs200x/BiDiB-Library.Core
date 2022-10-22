namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class LocalPongMessage : BiDiBOutputMessage
{
    public LocalPongMessage(byte[] address) : base(address, BiDiBMessage.MSG_LOCAL_PONG)
    {
    }
}
