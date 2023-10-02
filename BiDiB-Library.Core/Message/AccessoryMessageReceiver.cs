using System;
using System.ComponentModel.Composition;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core.Message;

[Export(typeof(IMessageReceiver))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class AccessoryMessageReceiver : IMessageReceiver
{
    private readonly IBiDiBNodesFactory nodesFactory;

    [ImportingConstructor]
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

        var accessory = Array.Find( node?.Accessories ?? Array.Empty<Accessory>(), x => x.Number == message.Number);
        if (accessory == null) { return; }

        accessory.ExecutionState = message.ExecutionState;
        accessory.ActiveAspect = message.Aspect;
    }
}