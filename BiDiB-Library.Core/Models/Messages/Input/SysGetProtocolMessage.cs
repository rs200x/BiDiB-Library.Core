namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_GET_P_VERSION)]
public class SysGetProtocolMessage : BiDiBInputMessage
{
    public SysGetProtocolMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_GET_P_VERSION, 0)
    {
    }
}