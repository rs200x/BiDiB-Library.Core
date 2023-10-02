using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Services.EventArgs;

public class BiDiBBoosterNodeAddedEventArgs : System.EventArgs
{
    public BiDiBBoosterNodeAddedEventArgs(BiDiBBoosterNode node)
    {
        Node = node;
    }

    public BiDiBBoosterNode Node { get; private set; }
}