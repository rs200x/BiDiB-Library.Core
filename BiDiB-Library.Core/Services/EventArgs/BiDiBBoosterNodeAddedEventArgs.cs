using org.bidib.netbidibc.core.Models;

namespace org.bidib.netbidibc.core.Services.EventArgs
{
    public class BiDiBBoosterNodeAddedEventArgs : System.EventArgs
    {
        public BiDiBBoosterNodeAddedEventArgs(BiDiBBoosterNode node)
        {
            Node = node;
        }

        public BiDiBBoosterNode Node { get; private set; }
    }
}