namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_FEATURE_COUNT)]
public class FeatureCountMessage : BiDiBInputMessage
{
    public FeatureCountMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_FEATURE_COUNT, 1)
    {
        Count = MessageParameters[0];
        StreamingSupported = MessageParameters.Length >= 2 && MessageParameters[1] == 1;
    }

    public byte Count { get; }

    public bool StreamingSupported { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, Count: {Count}, Streaming: {StreamingSupported}";
    }
}