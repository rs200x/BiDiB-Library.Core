using System.ComponentModel.Composition;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;
using StringMessage = org.bidib.Net.Core.Models.Messages.Input.StringMessage;

namespace org.bidib.Net.Core.Message;

[Export(typeof(IMessageReceiver))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class StringMessageReceiver : IMessageReceiver
{
    private readonly IBiDiBNodesFactory nodesFactory;
    private readonly ILogger<StringMessageReceiver> logger;

    [ImportingConstructor]
    public StringMessageReceiver(IBiDiBNodesFactory nodesFactory, ILogger<StringMessageReceiver> logger)
    {
        this.nodesFactory = nodesFactory;
        this.logger = logger;
    }

    public void ProcessMessage(BiDiBInputMessage message)
    {
        if (message == null) { return; }

        if (message.MessageType == BiDiBMessage.MSG_STRING)
        {
            HandleStringMessage(message as StringMessage);
        }
    }

    private void HandleStringMessage(StringMessage message)
    {
        if (message == null) { return; }

        var node = nodesFactory.GetNode(message.Address);
        if (node == null || message.Namespace != 0) { return; }

        if (message.StringId == 0)
        {
            logger.LogDebug("Set product name '{StringValue}' to {FullAddress}/{HexUniqueId}", message.StringValue, node.FullAddress, node.HexUniqueId);
            node.ProductName = message.StringValue.Trim();
        }

        if (message.StringId == 1)
        {
            logger.LogDebug("Set user name '{StringValue}' to {FullAddress}/{HexUniqueId}", message.StringValue, node.FullAddress, node.HexUniqueId);
            node.UserName = message.StringValue.Trim();
        }
    }
}