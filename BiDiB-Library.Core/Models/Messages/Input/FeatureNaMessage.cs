namespace org.bidib.netbidibc.core.Models.Messages.Input
{
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
}