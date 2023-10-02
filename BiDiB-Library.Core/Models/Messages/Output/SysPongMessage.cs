namespace org.bidib.Net.Core.Models.Messages.Output;

public class SysPongMessage : BiDiBOutputMessage
{
    public SysPongMessage(byte[] address) : base(address, BiDiBMessage.MSG_SYS_PONG)
    {
    }
}
