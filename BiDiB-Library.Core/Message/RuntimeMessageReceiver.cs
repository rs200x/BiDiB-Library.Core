using System;
using org.bidib.Net.Core.Models.Messages.Input;

namespace org.bidib.Net.Core.Message;

public class RuntimeMessageReceiver<TMessage> : IMessageReceiver where TMessage : BiDiBInputMessage
{
    private readonly Action<TMessage> messageAction;

    public RuntimeMessageReceiver(Action<TMessage> messageAction)
    {
        this.messageAction = messageAction;
    }

    public void ProcessMessage(BiDiBInputMessage message)
    {
        if (message != null && typeof(TMessage) == message.GetType())
        {
            messageAction.Invoke((TMessage)message);
        }
    }
}