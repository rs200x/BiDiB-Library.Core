using System.Collections.Generic;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class GuestRequestOutputMessage : BiDiBOutputMessage
{
    protected GuestRequestOutputMessage(byte[] address, TargetMode targetMode, byte[] targetId, BiDiBMessage messageType) 
        : base(address, messageType)
    {
        TargetMode = targetMode;
        TargetId = targetId;

        var baseParams = new List<byte> { (byte)TargetMode };

        if (TargetMode == TargetMode.BIDIB_TARGET_MODE_UID)
        {
            baseParams.AddRange(targetId);
        }

        BaseParams = baseParams;
    }

    public TargetMode TargetMode { get; }

    public byte[] TargetId { get; }

    protected IList<byte> BaseParams { get; }

    public override string ToString() => $"{base.ToString()}, {GetTarget()}";

    private string GetTarget()
    {
        var target = $"M: {TargetMode}";
        if (TargetId != null)
        {
            target += $", Id: {TargetId.GetDataString()}";
        }

        return target;
    }
}