using System.Linq;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class GuestRequestSendMessage : GuestRequestOutputMessage
{
    public GuestRequestSendMessage(byte[] address, TargetMode targetMode, byte[] targetId, BiDiBMessage targetMessageType) 
        : this(address, targetMode, targetId, targetMessageType, null)
    {
    }
    
    public GuestRequestSendMessage(byte[] address, TargetMode targetMode, byte[] targetId, BiDiBMessage targetMessageType, byte[] messageParams) 
        : base(address, targetMode, targetId, BiDiBMessage.MSG_GUEST_REQ_SEND)
    {
        TargetMessageType = targetMessageType;

        BaseParams.Add((byte)targetMessageType);

        if (messageParams != null)
        {
            MessageParams = messageParams;
            foreach (var messageParam in messageParams)
            {
                BaseParams.Add(messageParam);
            }
        }

        Parameters = BaseParams.ToArray();
    }

    public BiDiBMessage TargetMessageType { get; }

    public byte[] MessageParams { get; }

    public override string ToString() => $"{base.ToString()}, T: {TargetMessageType} P: {MessageParams.GetDataString()}";
}
