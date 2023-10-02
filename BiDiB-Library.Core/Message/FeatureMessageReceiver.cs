using System.ComponentModel.Composition;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core.Message;

[Export(typeof(IMessageReceiver))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class FeatureMessageReceiver : IMessageReceiver
{
    private readonly IBiDiBNodesFactory nodesFactory;

    [ImportingConstructor]
    public FeatureMessageReceiver(IBiDiBNodesFactory nodesFactory)
    {
        this.nodesFactory = nodesFactory;
    }

    public void ProcessMessage(BiDiBInputMessage message)
    {
        if(message == null) { return;}

        if (message.MessageType == BiDiBMessage.MSG_FEATURE)
        {
            HandleFeatureMessage(message as FeatureMessage);
        }
    }

    private void HandleFeatureMessage(FeatureMessage message)
    {
        if (message == null) { return; }

        var node = nodesFactory.GetNode(message.Address);

        var feature = node?.GetFeature(message.FeatureId);

        if (feature == null) { return; }

        feature.Value = message.Value;
        node.RaisePropertyChanged(()=> node.Features);
    }
}