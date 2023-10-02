using System.Collections;
using System.Linq;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_BM_MULTIPLE)]
public class FeedbackMultipleMessage : FeedbackMessage
{
    public FeedbackMultipleMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_MULTIPLE, 2)
    {
        StateSize = MessageParameters[1];
        PortStates = new BitArray(MessageParameters.Skip(2).ToArray());
    }

    public byte StateSize { get; }

    public BitArray PortStates { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, States: [{string.Join(",", PortStates.OfType<bool>().Select(x => x ? 1 : 0))}]";
    }
}