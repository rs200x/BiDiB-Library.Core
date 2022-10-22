namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class FeatureGetAllMessage : BiDiBOutputMessage
{
    public FeatureGetAllMessage(byte[] address, bool requestStreaming) : base(address, BiDiBMessage.MSG_FEATURE_GETALL)
    {

        Parameters = new[] { (byte) (requestStreaming ? 0x01 : 0x00) };
    }
}