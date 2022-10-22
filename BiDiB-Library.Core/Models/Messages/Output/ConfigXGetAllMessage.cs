using org.bidib.netbidibc.core.Models.BiDiB.Base;

namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class ConfigXGetAllMessage : BiDiBOutputMessage
    {
        public ConfigXGetAllMessage(byte[] address, PortType startType, byte startIndex, PortType endType, byte expectedItems)
            : base(address, BiDiBMessage.MSG_LC_CONFIGX_GET_ALL)
        {
            Parameters = new[] { (byte)startType, startIndex, (byte)endType, expectedItems };
        }
    }
}