namespace org.bidib.Net.Core.Models.Messages.Output;

public class LocalPongMessage : BiDiBOutputMessage
{
    public LocalPongMessage(byte[] address) : base(address, BiDiBMessage.MSG_LOCAL_PONG)
    {
    }
}
