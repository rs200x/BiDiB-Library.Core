using org.bidib.netbidibc.core.Utils;
using System.Linq;

namespace org.bidib.netbidibc.core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_GUEST_RESP_NOTIFY)]
public class GuestResponseNotifyMessage : GuestResponseInputMessage
{
    public GuestResponseNotifyMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_GUEST_RESP_NOTIFY, 3)
    {
        var targetSize = 1 + TargetId.Length;

        SubSequenceNumber = MessageParameters[targetSize];
        SubMessageType = (BiDiBMessage)MessageParameters[targetSize + 1];
        SubData = MessageParameters.Skip(targetSize + 2).ToArray();
    }

    public byte SubSequenceNumber { get; }

    public BiDiBMessage SubMessageType { get; }

    public byte[] SubData { get; }

    public override string ToString() => $"{base.ToString()}, subN: {SubSequenceNumber}, subT: {SubMessageType}, sd: {SubData.GetDataString()}";
}
