using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Utils;
using System;
using System.Linq;

namespace org.bidib.netbidibc.core.Models.Messages.Input;

public abstract class GuestResponseInputMessage : BiDiBInputMessage
{
    protected GuestResponseInputMessage(byte[] messageBytes, BiDiBMessage messageType, int expectedParameters)
        : base(messageBytes, messageType, expectedParameters)
    {
        TargetMode = (TargetMode)MessageParameters[0];
        TargetId = GetTargetId();
    }

    public TargetMode TargetMode { get; }

    public byte[] TargetId { get; }

    public override string ToString() => $"{base.ToString()}, M: {TargetMode}, Id: {TargetId.GetDataString()}";

    private byte[] GetTargetId()
    {
        switch (TargetMode)
        {
            case TargetMode.BIDIB_TARGET_MODE_UID: return MessageParameters[1..6];
            default: return Array.Empty<byte>();
        }
    }
}
