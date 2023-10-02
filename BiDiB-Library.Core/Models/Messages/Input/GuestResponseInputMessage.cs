using System;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

public abstract class GuestResponseInputMessage : BiDiBInputMessage
{
    protected GuestResponseInputMessage(byte[] messageBytes, BiDiBMessage messageType, int expectedParameters)
        : base(messageBytes, messageType, expectedParameters)
    {
        TargetMode = (TargetMode)MessageParameters[0];
        var uidExpected = TargetMode == TargetMode.BIDIB_TARGET_MODE_UID ||
                          MessageType != BiDiBMessage.MSG_GUEST_RESP_SUBSCRIPTION_COUNT;
        TargetId = uidExpected ? MessageParameters[1..6] : Array.Empty<byte>();
    }

    protected int DataIndex => 1 + TargetId.Length;

    public TargetMode TargetMode { get; }

    public byte[] TargetId { get; }

    public override string ToString() => $"{base.ToString()}, M: {TargetMode}, Id: {TargetId.GetDataString()}";
}
