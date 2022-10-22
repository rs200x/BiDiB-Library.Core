using org.bidib.netbidibc.core.Models;

namespace org.bidib.netbidibc.core.Services.EventArgs
{
    public class BiDiBNodeAddedEventArgs : System.EventArgs
    {
        public BiDiBNodeAddedEventArgs(BiDiBNode node)
        {
            Node = node;
        }

        public BiDiBNode Node { get; private set; }
    }
}