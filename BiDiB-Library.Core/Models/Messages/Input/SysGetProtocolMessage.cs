namespace org.bidib.netbidibc.core.Models.Messages.Input;

public class SysGetProtocolMessage : BiDiBInputMessage
{
    public SysGetProtocolMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_GET_P_VERSION, 0)
    {
    }
}