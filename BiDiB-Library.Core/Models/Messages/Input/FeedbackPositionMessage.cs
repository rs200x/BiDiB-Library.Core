using System;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_BM_POSITION)]
public class FeedbackPositionMessage : BiDiBInputMessage
{
    public FeedbackPositionMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_POSITION, 5)
    {

        FeedbackAddress = ByteUtils.GetDecoderAddress(MessageParameters[0], MessageParameters[1]);
        Direction = MessageParameters[1].GetDecoderDirection();

        FeedbackType = MessageParameters[2];

        LocationLow = MessageParameters[3];
        LocationHigh = MessageParameters[4];
        Location = BitConverter.ToUInt16(new[] { LocationLow, LocationHigh }, 0);
    }

    public ushort Location { get; set; }

    public byte LocationHigh { get; set; }

    public byte LocationLow { get; set; }

    public byte FeedbackType { get; set; }

    public ushort FeedbackAddress { get; set; }

    public DecoderDirection Direction { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, FAddr: {FeedbackAddress}/{Direction}, FLoc: {Location}, FType: {FeedbackType}";
    }
}