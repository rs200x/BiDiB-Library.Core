using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Services.EventArgs;

public class BiDiBBoosterNodeRemovedEventArgs : System.EventArgs
{
    public BiDiBBoosterNodeRemovedEventArgs(BiDiBBoosterNode node)
    {
        Node = node;
    }

    public BiDiBBoosterNode Node { get; private set; }
}