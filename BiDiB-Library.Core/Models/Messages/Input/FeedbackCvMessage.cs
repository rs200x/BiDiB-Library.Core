using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_BM_CV)]
public class FeedbackCvMessage : BiDiBInputMessage
{
    public FeedbackCvMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_CV, 5)
    {
        DecoderAddress = ByteUtils.GetDecoderAddress(MessageParameters[0], MessageParameters[1]);
        Direction = MessageParameters[1].GetDecoderDirection();
        CvLow = MessageParameters[2];
        CvHigh = MessageParameters[3];
        Data = MessageParameters[4];
    }

    public ushort DecoderAddress { get; }

    public DecoderDirection Direction { get; }

    public byte CvLow { get; }

    public byte CvHigh { get; }

    public int CvNumber => CvHigh * 256 + CvLow + 1;

    public byte Data { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, Addr: {DecoderAddress}/{Direction}, CV: {CvNumber}, Value: {Data}";
    }
}