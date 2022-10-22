namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class NodeNewMessage : NodeTabMessage
    {
        public NodeNewMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_NODE_NEW)
        {
        }
    }
}