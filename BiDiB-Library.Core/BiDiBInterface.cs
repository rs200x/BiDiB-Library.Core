using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Models.Messages.Output;
using org.bidib.netbidibc.core.Properties;
using org.bidib.netbidibc.core.Services.EventArgs;
using org.bidib.netbidibc.core.Services.Interfaces;

namespace org.bidib.netbidibc.core
{
    [Export(typeof(IBiDiBInterface))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class BiDiBInterface : IBiDiBInterface
    {
        private readonly ILogger<BiDiBInterface> logger;
        private readonly IConnectionService connectionService;
        private readonly IBiDiBMessageService messageService;
        private readonly IBiDiBMessageProcessor messageProcessor;
        private readonly IBiDiBNodesFactory nodesFactory;
        private readonly IBiDiBBoosterNodesManager boosterNodesManager;
        private readonly IEnumerable<IMessageReceiver> messageReceivers;
        private readonly ILoggerFactory loggerFactory;
        private ConnectionStateInfo connectionState;
        private bool isInitialized;

        [ImportingConstructor]
        public BiDiBInterface(
            IConnectionService connectionService,
            IBiDiBMessageService messageService,
            IBiDiBMessageProcessor messageProcessor,
            IBiDiBNodesFactory nodesFactory,
            IBiDiBBoosterNodesManager boosterNodesManager,
            [ImportMany]IEnumerable<IMessageReceiver> messageReceivers,
            ILoggerFactory loggerFactory)
        {
            this.connectionService = connectionService;
            this.messageService = messageService;
            this.messageProcessor = messageProcessor;
            this.nodesFactory = nodesFactory;
            this.boosterNodesManager = boosterNodesManager;
            this.messageReceivers = messageReceivers;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<BiDiBInterface>();

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
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                messageService.Activate();
                if (!await EstablishConnectionAsync(connectionConfig).ConfigureAwait(false))
                {
                    return;
                }

                if (ConnectionState.InterfaceState == InterfaceConnectionState.PartiallyConnected)
                {
                    messageService.Activate();
                    return;
                }

                BiDiBNode root = new BiDiBNode(messageProcessor, loggerFactory.CreateLogger<BiDiBNode>());

                if (connectionConfig.ConnectionType != InterfaceConnectionType.SerialOverTcp)
                {
                    root.Disable();
                }

                messageService.Activate();
                // wait for full system disable
                await Task.Delay(300).ConfigureAwait(false);

                int magic = root.GetMagic(5000);
                if (magic == 0)
                {
                    ConnectionState = new ConnectionStateInfo(InterfaceConnectionState.Disconnected, connectionConfig.ConnectionType, Resources.Error_NoResponseFromInterface);
                    messageService.Deactivate();
                    connectionService.CloseConnection();
                    return;
                }

                // valid connection established, start fresh
                RefreshNodes();
                root = nodesFactory.GetRootNode();
                root.Enable();
                ConnectionState = new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, connectionConfig.ConnectionType) { ConnectionName = connectionService.GetConnectionName() };
            }
            catch (Exception exception)
            {
                ConnectionState = new ConnectionStateInfo(InterfaceConnectionState.Disconnected, connectionConfig.ConnectionType, GetInterfaceError(connectionConfig));
                logger.LogError(exception, Resources.Error_CouldNotConnectToInterface);
                connectionService.CloseConnection();
            }
            finally
            {
                watch.Stop();
                logger.LogInformation($"Connection process finished after {watch.Elapsed:ss\\.fff}s");
            }
        }

        private async Task<bool> EstablishConnectionAsync(IConnectionConfig connectionConfig)
        {
            connectionService.ConfigureConnection(connectionConfig);
            ConnectionState = await connectionService.OpenConnectionAsync().ConfigureAwait(false);

            if (ConnectionState.InterfaceState == InterfaceConnectionState.Disconnected || ConnectionState.InterfaceState == InterfaceConnectionState.Unpaired)
            {
                if (ConnectionState.InterfaceState == InterfaceConnectionState.Disconnected)
                {
                    connectionService.CloseConnection();
                }

                return false;
            }

            if (ConnectionState.InterfaceState != InterfaceConnectionState.PartiallyConnected) { return true; }

            if (!(ConnectionState is NetBiDiBConnectionStateInfo netBiDiBStateInfo)) { return false; }

            var root = nodesFactory.CreateNode(new byte[] { 0 }, netBiDiBStateInfo.RemoteId);
            root.UserName = netBiDiBStateInfo.RemoteName;
            root.State = NodeState.Unavailable;
            return true;
        }

        private static string GetInterfaceError(IConnectionConfig connectionConfig)
        {
            string error = Resources.Error_CouldNotConnectToInterface;
            if (connectionConfig.ConnectionType == InterfaceConnectionType.SerialPort)
            {
                error = string.Format(CultureInfo.CurrentUICulture, Resources.Error_CouldNotConnectToSerialInterface, ((ISerialPortConfig)connectionConfig).Comport);
            }

            return error;
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
            messageService.Deactivate();
            connectionService.CloseConnection();
            messageService.Cleanup();
            nodesFactory.Reset();
            ConnectionState = new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialPort);
        }

        public void SendMessage(BiDiBMessage messageType, byte[] address, params byte[] parameters)
        {
            messageService.SendMessage(messageType, address, parameters);
        }

        public void SendMessage(BiDiBOutputMessage message)
        {
            messageProcessor.SendMessage(message);
        }

        public TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage) where TResponseMessage : BiDiBInputMessage
        {
           return messageProcessor.SendMessage<TResponseMessage>(outputMessage);
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
            connectionService.Dispose();
            messageService.Dispose();
        }
    }
}