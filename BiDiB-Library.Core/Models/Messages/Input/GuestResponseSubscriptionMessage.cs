using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_GUEST_RESP_SUBSCRIPTION)]
public class GuestResponseSubscriptionMessage : GuestResponseInputMessage
{
    public GuestResponseSubscriptionMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_GUEST_RESP_SUBSCRIPTION, 4)
    {
        var targetSize = 1 + TargetId.Length;
        Result = (SubscriptionResult)MessageParameters[targetSize];
        DownstreamSubscriptions = MessageParameters[targetSize+1];
        UpstreamSubscriptions = MessageParameters[targetSize+2];
    }

    public SubscriptionResult Result { get; }

    public byte DownstreamSubscriptions { get; }

    public byte UpstreamSubscriptions { get; }

    public override string ToString() => $"{base.ToString()}, R: {Result}, D: {DownstreamSubscriptions.GetBitString()}, U: {UpstreamSubscriptions.GetBitString()}";

}
