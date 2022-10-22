namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class FeatureMessage : BiDiBOutputMessage
    {
        public FeatureMessage(byte[] address, BiDiBFeature feature, byte value) : base(address, BiDiBMessage.MSG_FEATURE)
        {
            FeatureId = (byte)feature;
            Value = value;
            Parameters = new byte[] { FeatureId, Value };
        }

        public byte FeatureId { get;}

        public byte Value { get; }

        public override string ToString() => $"{base.ToString()}, FeatureId: {FeatureId}, Value: {Value}";
    }
}