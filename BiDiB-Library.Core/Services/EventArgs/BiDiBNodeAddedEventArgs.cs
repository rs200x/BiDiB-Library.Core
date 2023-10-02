using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Services.EventArgs;

public class BiDiBNodeAddedEventArgs : System.EventArgs
{
    public BiDiBNodeAddedEventArgs(BiDiBNode node)
    {
        Node = node;
    }

    public BiDiBNode Node { get; private set; }
}