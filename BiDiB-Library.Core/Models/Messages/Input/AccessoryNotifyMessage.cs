
namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_ACCESSORY_NOTIFY)]
public class AccessoryNotifyMessage : AccessoryStateMessage
{
    public AccessoryNotifyMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_ACCESSORY_NOTIFY)
    {
    }
}