namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_GET_SW_VERSION)]
public class SysGetSoftwareMessage : BiDiBInputMessage
{
    public SysGetSoftwareMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_GET_SW_VERSION, 0)
    {
    }
}