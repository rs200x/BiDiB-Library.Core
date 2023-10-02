namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_FEATURE)]
public class FeatureMessage : BiDiBInputMessage
{
    public FeatureMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_FEATURE,2)
    {
        FeatureId = MessageParameters[0];
        Value = MessageParameters[1];
    }

    public byte FeatureId { get;}

    public byte Value { get; }

    public override string ToString() => $"{base.ToString()}, FeatureId: {FeatureId}, Value: {Value}";
}