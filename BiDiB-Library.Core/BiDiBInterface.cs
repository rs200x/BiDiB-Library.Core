using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Models.Messages.Output;
using org.bidib.Net.Core.Properties;
using org.bidib.Net.Core.Services.EventArgs;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core;

[Export(typeof(IBiDiBInterface))]
[PartCreationPolicy(CreationPolicy.Shared)]
public sealed class BiDiBInterface : IBiDiBInterface
{
    private readonly IConnectionService connectionService;
    private readonly IBiDiBMessageService messageService;
    private readonly IBiDiBMessageProcessor messageProcessor;
    private readonly IBiDiBNodesFactory nodesFactory;
    private readonly IBiDiBBoosterNodesManager boosterNodesManager;
    private readonly ILogger<BiDiBInterface> logger;
    private readonly IEnumerable<IMessageReceiver> messageReceivers;
    private readonly IEnumerable<IConnectionStrategy> connectionStrategies;
    private ConnectionStateInfo connectionState;
    private bool isInitialized;
    private IConnectionStrategy currentConnectionStrategy;

    [ImportingConstructor]
    public BiDiBInterface(
        IConnectionService connectionService,
        IBiDiBMessageService messageService,
        IBiDiBMessageProcessor messageProcessor,
        IBiDiBNodesFactory nodesFactory,
        IBiDiBBoosterNodesManager boosterNodesManager,
        [ImportMany]IEnumerable<IMessageReceiver> messageReceivers,
        [ImportMany]IEnumerable<IConnectionStrategy> connectionStrategies,
        ILogger<BiDiBInterface> logger)
    {
        this.connectionService = connectionService;
        this.messageService = messageService;
        this.messageProcessor = messageProcessor;
        this.nodesFactory = nodesFactory;
        this.boosterNodesManager = boosterNodesManager;

        this.messageReceivers = messageReceivers;
        this.connectionStrategies = connectionStrategies;

        this.logger = logger;

        ConnectionState = new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialPort);
    }

    public void Initialize()
    {
        if (isInitialized)
        {
            return;
        }

        connectionService.ConnectionClosed += HandleConnectionClosed;
        connectionService.ConnectionStateChanged += HandleConnectionStateChanged;

        nodesFactory.NodeAdded = HandleNodesFactoryNodeAdded;
        nodesFactory.NodeRemoved = HandleNodesFactoryNodeRemoved;
        nodesFactory.MessageProcessor = messageProcessor;

        boosterNodesManager.BoosterAdded = b => BoosterAdded?.Invoke(this, new BiDiBBoosterNodeAddedEventArgs(b));
        boosterNodesManager.BoosterRemoved = b => BoosterRemoved?.Invoke(this, new BiDiBBoosterNodeRemovedEventArgs(b));

        foreach (var messageReceiver in messageReceivers)
        {
            messageService.Register(messageReceiver);
        }

        isInitialized = true;
    }

    public async Task ConnectAsync(IConnectionConfig connectionConfig)
    {
        currentConnectionStrategy = connectionStrategies.FirstOrDefault(x => x.Type == connectionConfig.ConnectionStrategyType);
        if (currentConnectionStrategy == null)
        {
            logger.LogWarning("No connection strategy found for type {Strategy}", connectionConfig.ConnectionStrategyType);
            return;
        }

        await currentConnectionStrategy.ConnectAsync(connectionConfig);
        ConnectionState = currentConnectionStrategy.ConnectionState;
    }

    public void RefreshNodes()
    {
        messageService.Cleanup();
        nodesFactory.Reset();
    }

    public Task DisconnectAsync()
    {
        return Task.Factory.StartNew(DisconnectCore);
    }

    public void RejectConnection()
    {
        connectionService.RejectConnection();
    }

    public void LinkConnection()
    {
        connectionService.LinkConnection();
    }

    private void DisconnectCore()
    {
        currentConnectionStrategy.Disconnect();
        ConnectionState = currentConnectionStrategy.ConnectionState;
    }

    public void SendMessage(BiDiBMessage messageType, byte[] address, params byte[] parameters)
    {
        messageService.SendMessage(messageType, address, parameters);
    }

    public void SendMessage(BiDiBOutputMessage message)
    {
        messageProcessor.SendMessage(message);
    }

    public TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage, int timeout = 500) where TResponseMessage : BiDiBInputMessage
    {
        return messageProcessor.SendMessage<TResponseMessage>(outputMessage, timeout);
    }

    public void Register(IMessageReceiver messageReceiver)
    {
        messageService.Register(messageReceiver);
    }

    public void Unregister(IMessageReceiver messageReceiver)
    {
        messageService.Unregister(messageReceiver);
    }

    private void HandleNodesFactoryNodeAdded(BiDiBNode newNode)
    {
        if (newNode == null) { return; }

        newNode.GetMagic(1500);
        newNode.GetProtocolVersion();
        newNode.GetSoftwareVersion();

        if (newNode.UniqueId == 0)
        {
            newNode.GetUniqueId();
        }

        newNode.GetFeatures();

        NodeAdded?.Invoke(this, new BiDiBNodeAddedEventArgs(newNode));
        boosterNodesManager.NodeAdded(newNode);

        if (newNode.HasSubNodesFunctions)
        {
            // find sub nodes
            messageProcessor.GetChildNodes(newNode.Address);
        }

        var root = nodesFactory.GetRootNode();
        if (root != newNode && root.IsEnabled)
        {
            newNode.Enable();
        }
    }

    private void HandleNodesFactoryNodeRemoved(BiDiBNode removedNode)
    {
        if (removedNode == null) { return; }

        NodeRemoved?.Invoke(this, new BiDiBNodeRemovedEventArgs(removedNode));

        boosterNodesManager.NodeRemoved(removedNode);
    }

    private void HandleConnectionClosed(object sender, EventArgs eventArgs)
    {
        DisconnectCore();
        ConnectionState = new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialPort, Resources.ConnectionClosedByRemote);
    }

    private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
    {
        ConnectionState = e.StateInfo;
    }

    public void Cleanup()
    {
        nodesFactory.NodeAdded = null;
        nodesFactory.NodeRemoved = null;
    }

    public ConnectionStateInfo ConnectionState
    {
        get => connectionState;
        private set
        {
            connectionState = value;
            OnConnectionStatusChanged(connectionState);
        }
    }

    public IEnumerable<BiDiBNode> Nodes => nodesFactory.Nodes;

    public IEnumerable<BiDiBBoosterNode> Boosters => boosterNodesManager.Boosters;

    public event EventHandler<BiDiBNodeAddedEventArgs> NodeAdded;
    public event EventHandler<BiDiBBoosterNodeAddedEventArgs> BoosterAdded;
    public event EventHandler<BiDiBNodeRemovedEventArgs> NodeRemoved;
    public event EventHandler<BiDiBBoosterNodeRemovedEventArgs> BoosterRemoved;
    public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStatusChanged;

    private void OnConnectionStatusChanged(ConnectionStateInfo stateInfo)
    {
        ConnectionStatusChanged?.Invoke(this, new ConnectionStateChangedEventArgs(stateInfo));
    }

    public void Dispose()
    {
        connectionService.ConnectionClosed -= HandleConnectionClosed;
        connectionService.ConnectionStateChanged -= HandleConnectionStateChanged;
    }
}