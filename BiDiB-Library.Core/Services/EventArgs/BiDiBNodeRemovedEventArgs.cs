using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Services.EventArgs;

public class BiDiBNodeRemovedEventArgs : System.EventArgs
{
    public BiDiBNodeRemovedEventArgs(BiDiBNode node)
    {
        Node = node;
    }

    public BiDiBNode Node { get; private set; }
}