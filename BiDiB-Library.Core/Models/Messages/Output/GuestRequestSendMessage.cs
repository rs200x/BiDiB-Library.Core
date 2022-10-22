using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Utils;
using System.Collections.Generic;

namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class GuestRequestSendMessage : BiDiBOutputMessage
{
    public GuestRequestSendMessage(byte[] address, TargetMode targetMode, byte[] nodeId, BiDiBMessage targetMessageType, byte[] messageParams = null) : base(address, BiDiBMessage.MSG_GUEST_REQ_SEND)
    {
        TargetMode = targetMode;
        TargetMessageType = targetMessageType;
        NodeId = nodeId;

        var parameters = new List<byte> { (byte)targetMode };

        if(TargetMode == TargetMode.BIDIB_TARGET_MODE_UID)
        {
            parameters.AddRange(nodeId);
        }

        
        parameters.Add((byte)targetMessageType);

        if (messageParams != null)
        {
            parameters.AddRange(messageParams);
        }

        Parameters = parameters.ToArray();
    }

    public TargetMode TargetMode { get; }

    public BiDiBMessage TargetMessageType { get; }

    public byte[] NodeId { get; }

    public override string ToString() => $"{base.ToString()}, M: {TargetMode}, T: {TargetMessageType} NID: {NodeId.GetDataString()}";
}
