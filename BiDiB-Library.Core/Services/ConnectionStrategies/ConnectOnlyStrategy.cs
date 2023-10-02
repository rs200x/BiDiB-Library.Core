using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core.Services.ConnectionStrategies;

[Export(typeof(IConnectionStrategy))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class ConnectOnlyStrategy : ConnectionStrategyBase, IConnectionStrategy
{

    private readonly IConnectionService connectionService;
    private readonly IBiDiBMessageService messageService;
    private readonly ILogger<ConnectOnlyStrategy> logger;

    [ImportingConstructor]
    public ConnectOnlyStrategy(
        IConnectionService connectionService,
        IBiDiBMessageService messageService,
        IBiDiBMessageProcessor messageProcessor,
        IBiDiBNodesFactory nodesFactory,
        ILoggerFactory loggerFactory
    ):base(connectionService, messageService)
    {
        this.connectionService = connectionService;
        this.messageService = messageService;
        logger = loggerFactory.CreateLogger<ConnectOnlyStrategy>();
    }

    public override ConnectionStrategyType Type => ConnectionStrategyType.ConnectOnly;

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
            await EstablishConnectionAsync(connectionConfig);
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
}