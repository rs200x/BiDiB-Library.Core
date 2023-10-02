using System.Linq;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class GuestRequestUnsubscribeMessage : GuestRequestOutputMessage
{
    public GuestRequestUnsubscribeMessage(byte[] address, TargetMode targetMode, byte[] targetId, byte downstream, byte upstream)
        : base(address, targetMode, targetId, BiDiBMessage.MSG_GUEST_REQ_UNSUBSCRIBE)
    {
        DownstreamSubscriptions = downstream;
        UpstreamSubscriptions = upstream;

        BaseParams.Add(DownstreamSubscriptions);
        BaseParams.Add(UpstreamSubscriptions);

        Parameters = BaseParams.ToArray();
    }


    public byte DownstreamSubscriptions { get; }

    public byte UpstreamSubscriptions { get; }

    public override string ToString() => $"{base.ToString()}, D: {DownstreamSubscriptions.GetBitString()}, U: {UpstreamSubscriptions.GetBitString()}";
}
