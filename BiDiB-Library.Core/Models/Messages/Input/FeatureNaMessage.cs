namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_FEATURE_NA)]
public class FeatureNaMessage : BiDiBInputMessage
{
    public FeatureNaMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_FEATURE_NA, 1)
    {
        FeatureId = MessageParameters[0];
    }

    public int FeatureId { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, FeatureId: {FeatureId}";
    }
}