namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_STALL)]
public class NodeStallMessage : BiDiBInputMessage
{
    public NodeStallMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_STALL, 1)
    {

        StallActive = MessageParameters[0] == 1;
    }

    public bool StallActive { get;  }

    public override string ToString() => $"{base.ToString()}, Stall:{StallActive}";
}