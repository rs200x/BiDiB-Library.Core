namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_NODETAB_COUNT)]
public class NodeTabCountMessage : BiDiBInputMessage
{
    public NodeTabCountMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_NODETAB_COUNT, 1)
    {
        NodeCount = MessageParameters[0];
    }

    public byte NodeCount { get; }

    public override string ToString() => $"{base.ToString()}, Count:{NodeCount}";
}