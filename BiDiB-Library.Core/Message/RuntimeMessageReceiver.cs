using System;
using org.bidib.netbidibc.core.Models.Messages.Input;

namespace org.bidib.netbidibc.core.Message
{
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
}