namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class FeatureNextMessage : BiDiBOutputMessage
    {
        public FeatureNextMessage(byte[] address) : base(address, BiDiBMessage.MSG_FEATURE_GETNEXT)
        {
        }
    }
}