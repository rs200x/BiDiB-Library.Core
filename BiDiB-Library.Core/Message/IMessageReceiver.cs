using org.bidib.Net.Core.Models.Messages.Input;

namespace org.bidib.Net.Core.Message;

public interface IMessageReceiver
{
    void ProcessMessage(BiDiBInputMessage message);
}