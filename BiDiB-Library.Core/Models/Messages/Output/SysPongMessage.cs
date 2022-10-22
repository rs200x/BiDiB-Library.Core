namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class SysPongMessage : BiDiBOutputMessage
{
    public SysPongMessage(byte[] address) : base(address, BiDiBMessage.MSG_SYS_PONG)
    {
    }
}
