namespace org.bidib.netbidibc.core.Models.Messages.Input;

public class SysGetSoftwareMessage : BiDiBInputMessage
{
    public SysGetSoftwareMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_GET_SW_VERSION, 0)
    {
    }
}