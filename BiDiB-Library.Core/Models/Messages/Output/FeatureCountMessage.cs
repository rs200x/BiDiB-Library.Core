namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class FeatureCountMessage : BiDiBOutputMessage
{
    public FeatureCountMessage(byte[] address, byte count, bool streamingSupported) : base(address, BiDiBMessage.MSG_FEATURE_COUNT)
    {
        Count = count;
        StreamingSupported = streamingSupported;
        Parameters = new byte[] { Count, (byte) (StreamingSupported ? 0x01 : 0x00) };
    }

    public byte Count { get; }

    public bool StreamingSupported { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, Count: {Count}, Streaming: {StreamingSupported}";
    }
}