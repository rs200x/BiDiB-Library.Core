using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Controllers
{
    public class SerialOverTcpControllerFactory : IConnectionControllerFactory
    {
        private readonly ILoggerFactory loggerFactory;

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
}