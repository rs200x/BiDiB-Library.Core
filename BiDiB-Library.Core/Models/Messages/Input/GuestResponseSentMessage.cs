namespace org.bidib.netbidibc.core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_GUEST_RESP_SENT)]
public class GuestResponseSentMessage : GuestResponseInputMessage
{
    public GuestResponseSentMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_GUEST_RESP_SENT, 3)
    {
    }
}
