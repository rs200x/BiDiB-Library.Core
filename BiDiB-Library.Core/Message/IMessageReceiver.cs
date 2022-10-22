using org.bidib.netbidibc.core.Models.Messages.Input;

namespace org.bidib.netbidibc.core.Message
{
    public interface IMessageReceiver
    {
        void ProcessMessage(BiDiBInputMessage message);
    }
}