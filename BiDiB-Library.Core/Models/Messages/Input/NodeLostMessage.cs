namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class NodeLostMessage : NodeTabMessage
    {
        public NodeLostMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_NODE_LOST)
        {
        }
    }
}