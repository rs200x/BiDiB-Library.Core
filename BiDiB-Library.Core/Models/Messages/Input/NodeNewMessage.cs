namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_NODE_NEW)]
public class NodeNewMessage : NodeTabMessage
{
    public NodeNewMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_NODE_NEW)
    {
    }
}