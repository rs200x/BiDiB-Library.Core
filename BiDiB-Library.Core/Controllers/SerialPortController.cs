using System;
using System.IO;
using System.IO.Ports;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Properties;
using System.Globalization;
using System.Threading.Tasks;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Utils;
using Microsoft.Extensions.Logging;

namespace org.bidib.netbidibc.core.Controllers
{
    internal class SerialPortController : ConnectionController<ISerialPortConfig>, ISerialPortController
    {
        private readonly ISerialPort serialPort;
        private readonly ILogger<SerialPortController> logger;
        private readonly ILogger rawLogger;

        private sealed class SerialPortWrapper : SerialPort, ISerialPort
        {

        }

        public SerialPortController(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<SerialPortController>();
            rawLogger = loggerFactory.CreateLogger("RAW");

            serialPort = new SerialPortWrapper
            {
                ReadBufferSize = 8192,
                WriteBufferSize = 4096,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,

                Handshake = Handshake.None,
                WriteTimeout = 200
            };
        }

        public override ConnectionStateInfo ConnectionState => serialPort is {IsOpen: true}
            ? new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialPort)
            : new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialPort);

        public override string ConnectionName => serialPort.PortName;

        public override void Initialize(ISerialPortConfig config)
        {
            // close if open, just in case
            Close();

            if (config.Baudrate != 19200 && config.Baudrate != 115200 && config.Baudrate != 1_000_000)
            {
                throw new InvalidOperationException("%%ERROR: Please define COM-Port and baud rate !!!!\n");
            }

            serialPort.PortName = config.Comport;
            serialPort.BaudRate = config.Baudrate;
            logger.LogInformation($"BiDiB at {serialPort.PortName} with {serialPort.BaudRate} Baud");
        }

        private void HandleSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytes = serialPort.BytesToRead;
            byte[] comBuffer = new byte[bytes];
            serialPort.Read(comBuffer, 0, bytes);

            rawLogger.LogInformation($"<<< {comBuffer.GetDataString()}");
            ProcessReceivedData?.Invoke(comBuffer);
        }

        private void HandleSerialPinChangedEvent(object sender, SerialPinChangedEventArgs e)
        {
            logger.LogInformation($"Received serial pin change event: {e}");
            if (e.EventType != SerialPinChange.CtsChanged) { return; }
            
            try
            {
                logger.LogError($"The CTS pin has changed: {serialPort.CtsHolding}");
            }
            catch (InvalidOperationException exception)
            {
                logger.LogWarning($"The CTS pin has changed, but state could not be determined. {e}, {exception.InnerException?.Message}");
            }
        }

        public override bool SendMessage(byte[] messageBytes, int byteCount)
        {
            if (serialPort.IsOpen)
            {
                byte[] message = new byte[byteCount];
                Array.Copy(messageBytes, 0, message, 0, byteCount);
                rawLogger.LogInformation($">>> {message.GetDataString()}");
                serialPort.Write(messageBytes, 0, byteCount);
            }
            else
            {
                logger.LogError(Resources.Error_SerialPortNotConnected);
                return false;
            }

            return true;
        }

        public override Task<ConnectionStateInfo> OpenConnectionAsync()
        {
            return Task.Factory.StartNew(OpenPort);
        }

        private ConnectionStateInfo OpenPort()
        {
            if (serialPort.IsOpen) { return new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialPort); }

            string error = string.Empty;
            InterfaceConnectionState interfaceState;

            try
            {
                serialPort.PinChanged += HandleSerialPinChangedEvent;
                serialPort.DataReceived += HandleSerialPortDataReceived;
                serialPort.Open();
                serialPort.RtsEnable = true;

                interfaceState = InterfaceConnectionState.FullyConnected;

                bool ctsHolding = serialPort.CtsHolding;
                logger.LogInformation($"The current CTS pin: {ctsHolding}");
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                error = string.Format(CultureInfo.CurrentUICulture, Resources.Error_CouldNotConnectToSerialInterface, serialPort.PortName);
                logger.LogError(ex, string.Format(CultureInfo.CurrentUICulture, Resources.SerialPortConnectionError, serialPort.PortName));
                interfaceState = InterfaceConnectionState.Disconnected;
            }

            return new ConnectionStateInfo(interfaceState, InterfaceConnectionType.SerialPort, error);
        }

        public override void Close()
        {
            serialPort.DataReceived -= HandleSerialPortDataReceived;
            serialPort.PinChanged -= HandleSerialPinChangedEvent;

            if (!serialPort.IsOpen) { return; }

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            serialPort.Close();
        }
    }
}