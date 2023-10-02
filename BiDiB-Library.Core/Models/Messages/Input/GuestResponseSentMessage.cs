using System.Linq;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_GUEST_RESP_SENT)]
public class GuestResponseSentMessage : GuestResponseInputMessage
{
    public GuestResponseSentMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_GUEST_RESP_SENT, 6 + 2)
    {
        AckSequenceNumber = MessageParameters[DataIndex];
        Result = (RequestResult)MessageParameters[DataIndex + 1];
        Details = MessageParameters.Skip(DataIndex + 1).ToArray();
    }

    public byte AckSequenceNumber { get; }

    public RequestResult Result { get; }

    public byte[] Details { get; }

    public override string ToString() => $"{base.ToString()}, AS: {AckSequenceNumber}, R: {Result}, D: {Details.GetDataString()}";
}
