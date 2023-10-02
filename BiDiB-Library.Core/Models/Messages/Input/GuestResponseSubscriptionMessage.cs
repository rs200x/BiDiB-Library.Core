using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_GUEST_RESP_SUBSCRIPTION)]
public class GuestResponseSubscriptionMessage : GuestResponseInputMessage
{
    public GuestResponseSubscriptionMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_GUEST_RESP_SUBSCRIPTION, 6 + 3)
    {
        var result = MessageParameters[DataIndex];
        MultiNodeResolution = (result & 1) == 1;
        Result = (SubscriptionResult)(result >> 1);
        DownstreamSubscriptions = MessageParameters[DataIndex + 1];
        UpstreamSubscriptions = MessageParameters[DataIndex + 2];
    }

    public bool MultiNodeResolution { get; }

    public SubscriptionResult Result { get; }

    public byte DownstreamSubscriptions { get; }

    public byte UpstreamSubscriptions { get; }

    public override string ToString() => $"{base.ToString()}, R: {Result}, M {MultiNodeResolution}, D: {DownstreamSubscriptions.GetBitString()}, U: {UpstreamSubscriptions.GetBitString()}";

}
