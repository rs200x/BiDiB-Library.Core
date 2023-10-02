namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_NODE_LOST)]
public class NodeLostMessage : NodeTabMessage
{
    public NodeLostMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_NODE_LOST)
    {
    }
}