using org.bidib.netbidibc.core.Models;

namespace org.bidib.netbidibc.core.Services.EventArgs
{
    public class BiDiBNodeRemovedEventArgs : System.EventArgs
    {
        public BiDiBNodeRemovedEventArgs(BiDiBNode node)
        {
            Node = node;
        }

        public BiDiBNode Node { get; private set; }
    }
}