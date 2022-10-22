using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Controllers
{
    public class SerialPortControllerFactory : IConnectionControllerFactory
    {
        private readonly ILoggerFactory loggerFactory;

        public SerialPortControllerFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public InterfaceConnectionType ConnectionType => InterfaceConnectionType.SerialPort;

        /// <inheritdoc />
        public IConnectionController GetController(IConnectionConfig connectionConfig)
        {
            var controller = new SerialPortController(loggerFactory);
            controller.Initialize(connectionConfig as ISerialPortConfig);

            return controller;
        }
    }
}