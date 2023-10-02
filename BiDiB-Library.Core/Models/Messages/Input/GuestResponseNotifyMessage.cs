using System.Linq;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_GUEST_RESP_NOTIFY)]
public class GuestResponseNotifyMessage : GuestResponseInputMessage
{
    public GuestResponseNotifyMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_GUEST_RESP_NOTIFY, 6 + 2)
    {
        SubSequenceNumber = MessageParameters[DataIndex];
        SubMessageType = (BiDiBMessage)MessageParameters[DataIndex + 1];
        SubData = MessageParameters.Skip(DataIndex + 2).ToArray();
    }

    public byte SubSequenceNumber { get; }

    public BiDiBMessage SubMessageType { get; }

    public byte[] SubData { get; }

    public override string ToString() => $"{base.ToString()}, subN: {SubSequenceNumber}, subT: {SubMessageType}, sd: {SubData.GetDataString()}";
}
