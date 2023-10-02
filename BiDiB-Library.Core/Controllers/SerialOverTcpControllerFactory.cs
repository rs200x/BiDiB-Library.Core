using System.ComponentModel.Composition;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Controllers;

[Export(typeof(IConnectionControllerFactory))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class SerialOverTcpControllerFactory : IConnectionControllerFactory
{
    private readonly ILoggerFactory loggerFactory;

    [ImportingConstructor]
    public SerialOverTcpControllerFactory(ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public InterfaceConnectionType ConnectionType => InterfaceConnectionType.SerialOverTcp;

    /// <inheritdoc />
    public IConnectionController GetController(IConnectionConfig connectionConfig)
    {
        var controller = new SocketController(loggerFactory);
        controller.Initialize(connectionConfig as INetConfig);

        return controller;
    }
}