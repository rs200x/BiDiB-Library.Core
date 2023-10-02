using System;
using System.Linq;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_GUEST_RESP_SUBSCRIPTION_COUNT)]
public class GuestResponseSubscriptionCountMessage : GuestResponseInputMessage
{
    public GuestResponseSubscriptionCountMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_GUEST_RESP_SUBSCRIPTION_COUNT, 1 + 2)
    {
        Count = BitConverter.ToInt16(MessageParameters.Skip(DataIndex).ToArray(), 0);
    }

    public int Count { get; }

    public override string ToString() => $"{base.ToString()}, C: {Count}";
}
