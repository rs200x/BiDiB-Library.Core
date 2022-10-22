using System.Linq;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Services.Interfaces;

namespace org.bidib.netbidibc.core.Message
{
    public class AccessoryMessageReceiver : IMessageReceiver
    {
        private readonly IBiDiBNodesFactory nodesFactory;

        public AccessoryMessageReceiver(IBiDiBNodesFactory nodesFactory)
        {
            this.nodesFactory = nodesFactory;
        }

        public void ProcessMessage(BiDiBInputMessage message)
        {
            if (message == null) { return; }

            if (message.MessageType == BiDiBMessage.MSG_ACCESSORY_STATE)
            {
                HandleAccessoryState(message as AccessoryStateMessage);
            }
        }

        private void HandleAccessoryState(AccessoryStateMessage message)
        {
            if (message == null) { return; }

            var node = nodesFactory.GetNode(message.Address);

            var accessory = node?.Accessories?.FirstOrDefault(x => x.Number == message.Number);
            if (accessory == null) { return; }

            accessory.ExecutionState = message.ExecutionState;
            accessory.ActiveAspect = message.Aspect;
        }
    }
}