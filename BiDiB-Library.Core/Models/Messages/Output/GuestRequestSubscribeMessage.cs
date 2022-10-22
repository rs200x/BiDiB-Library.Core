using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Utils;
using System.Collections.Generic;

namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class GuestRequestSubscribeMessage : BiDiBOutputMessage
{
    public GuestRequestSubscribeMessage(byte[] address, TargetMode targetMode, byte[] targetId, byte downstream, byte upstream)
        : base(address, BiDiBMessage.MSG_GUEST_REQ_SUBSCRIBE)
    {
        TargetMode = targetMode;
        DownstreamSubscriptions = downstream;
        UpstreamSubscriptions = upstream;

        var parameters = new List<byte>
        {
            (byte)targetMode
        };

        if (targetId != null)
        {
            parameters.AddRange(targetId);
        }

        parameters.Add(downstream);
        parameters.Add(upstream);

        Parameters = parameters.ToArray();
    }

    public TargetMode TargetMode { get; }

    public byte DownstreamSubscriptions { get; }
    public byte UpstreamSubscriptions { get; }

    public override string ToString() => $"{base.ToString()}, M: {TargetMode}, D: {DownstreamSubscriptions.GetBitString()}, U: {UpstreamSubscriptions.GetBitString()}";
}
