using org.bidib.netbidibc.core.Models;

namespace org.bidib.netbidibc.core.Services.EventArgs
{
    public class BiDiBBoosterNodeRemovedEventArgs : System.EventArgs
    {
        public BiDiBBoosterNodeRemovedEventArgs(BiDiBBoosterNode node)
        {
            Node = node;
        }

        public BiDiBBoosterNode Node { get; private set; }
    }
}