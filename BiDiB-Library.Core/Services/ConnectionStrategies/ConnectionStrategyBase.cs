using System.Globalization;
using System.Threading.Tasks;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Properties;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core.Services.ConnectionStrategies;

public abstract class ConnectionStrategyBase : IConnectionStrategy
{
    private readonly IConnectionService connectionService;
    private readonly IBiDiBMessageService messageService;


    protected ConnectionStrategyBase(
        IConnectionService connectionService,
        IBiDiBMessageService messageService)
    {
        this.connectionService = connectionService;
        this.messageService = messageService;
    }

    public ConnectionStateInfo ConnectionState { get; protected set; } = new(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialPort);

    public abstract ConnectionStrategyType Type { get; }

    public abstract Task ConnectAsync(IConnectionConfig connectionConfig);

    protected async Task<bool> EstablishConnectionAsync(IConnectionConfig connectionConfig)
    {
        connectionService.ConfigureConnection(connectionConfig);
        ConnectionState = await connectionService.OpenConnectionAsync();

        if (ConnectionState.InterfaceState is InterfaceConnectionState.Disconnected or InterfaceConnectionState.Unpaired)
        {
            if (ConnectionState.InterfaceState == InterfaceConnectionState.Disconnected)
            {
                connectionService.CloseConnection();
            }

            return false;
        }

        if (ConnectionState.InterfaceState != InterfaceConnectionState.PartiallyConnected) { return true; }

        return ConnectionState.InterfaceType == InterfaceConnectionType.NetBiDiB;
    }

    protected static string GetInterfaceError(IConnectionConfig connectionConfig)
    {
        if (connectionConfig == null)
        {
            return string.Empty;
        }

        var error = Resources.Error_CouldNotConnectToInterface;
        if (connectionConfig.ConnectionType == InterfaceConnectionType.SerialPort)
        {
            error = string.Format(CultureInfo.CurrentUICulture, Resources.Error_CouldNotConnectToSerialInterface, ((ISerialPortConfig)connectionConfig).Comport);
        }

        return error;
    }

    public virtual void Disconnect()
    {
        messageService.Deactivate();
        connectionService.CloseConnection();
        messageService.Cleanup();
        ConnectionState = new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.Unknown);
    }
}