using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Properties;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core.Services.ConnectionStrategies;

[Export(typeof(IConnectionStrategy))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class DefaultStrategy : ConnectionStrategyBase, IConnectionStrategy
{
    private readonly IConnectionService connectionService;
    private readonly IBiDiBMessageService messageService;
    private readonly IBiDiBMessageProcessor messageProcessor;
    private readonly IBiDiBNodesFactory nodesFactory;
    private readonly ILoggerFactory loggerFactory;

    private readonly ILogger<DefaultStrategy> logger;

    private const int RootInitDelay = 300;

    [ImportingConstructor]
    public DefaultStrategy(
        IConnectionService connectionService,
        IBiDiBMessageService messageService,
        IBiDiBMessageProcessor messageProcessor,
        IBiDiBNodesFactory nodesFactory,
        ILoggerFactory loggerFactory
        ):base(connectionService, messageService)
    {
        this.connectionService = connectionService;
        this.messageService = messageService;
        this.messageProcessor = messageProcessor;
        this.nodesFactory = nodesFactory;
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<DefaultStrategy>();
    }

    public override ConnectionStrategyType Type => ConnectionStrategyType.Default;

    public override async Task ConnectAsync(IConnectionConfig connectionConfig)
    {
        if (connectionConfig == null)
        {
            throw new ArgumentNullException(nameof(connectionConfig));
        }

        await ConnectInternalAsync(connectionConfig);
    }

    private async Task ConnectInternalAsync(IConnectionConfig connectionConfig)
    {
        var watch = Stopwatch.StartNew();
        try
        {
            messageService.Activate();
            if (!await EstablishConnectionAsync(connectionConfig))
            {
                return;
            }

            if (ConnectionState.InterfaceState == InterfaceConnectionState.PartiallyConnected)
            {
                return;
            }

            var root = new BiDiBNode(messageProcessor, loggerFactory.CreateLogger<BiDiBNode>());

            if (connectionConfig.ConnectionType != InterfaceConnectionType.SerialOverTcp)
            {
                root.Disable();
            }

            // wait for full system disable
            await Task.Delay(RootInitDelay);

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
            logger.LogError(exception, "Could not connect to interface");
            connectionService.CloseConnection();
        }
        finally
        {
            watch.Stop();
            logger.LogInformation("Connection process finished after {Time:ss\\.fff}s", watch.Elapsed);
        }
    }

    public override void Disconnect()
    {
        base.Disconnect();

        nodesFactory.Reset();
    }

    private void RefreshNodes()
    {
        messageService.Cleanup();
        nodesFactory.Reset();
    }
}