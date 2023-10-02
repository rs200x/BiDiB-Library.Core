namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_GET_MAGIC)]
public class SysGetMagicMessage : BiDiBInputMessage
{
    public SysGetMagicMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_GET_MAGIC, 0)
    {
    }
}