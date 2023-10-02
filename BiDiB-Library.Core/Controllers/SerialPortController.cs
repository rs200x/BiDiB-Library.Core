using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Properties;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Controllers;

[Localizable(false)]
public class SerialPortController : ConnectionController<ISerialPortConfig>, ISerialPortController
{
    private readonly ISerialPort serialPort;
    private readonly ILogger<SerialPortController> logger;
    private readonly ILogger rawLogger;

    private sealed class SerialPortWrapper : SerialPort, ISerialPort
    {

    }

    public SerialPortController(ILoggerFactory loggerFactory)
    {
        if (loggerFactory == null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        logger = loggerFactory.CreateLogger<SerialPortController>();
        rawLogger = loggerFactory.CreateLogger(BiDiBConstants.LoggerContextRaw);

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

    public override ConnectionStateInfo ConnectionState => serialPort is { IsOpen: true }
        ? new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialPort)
        : new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialPort);

    public override string ConnectionName => serialPort.PortName;

    public override void Initialize(ISerialPortConfig config)
    {
        // close if open, just in case
        Close();

        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        if (config.Baudrate != 19200 && config.Baudrate != 115200 && config.Baudrate != 1_000_000)
        {
            throw new InvalidOperationException($"Invalid baud rate configuration {config.Baudrate}! Valid values are 19.200 or 115.200");
        }

        serialPort.PortName = config.Comport;
        serialPort.BaudRate = config.Baudrate;
        logger.LogInformation("BiDiB at {PortName} with {BaudRate} Baud", serialPort.PortName, serialPort.BaudRate);
    }

    private void HandleSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var bytes = serialPort.BytesToRead;
        var comBuffer = new byte[bytes];
        serialPort.Read(comBuffer, 0, bytes);

        rawLogger.LogInformation("<<< {Data}", comBuffer.GetDataString());
        ProcessReceivedData?.Invoke(comBuffer);
    }

    private void HandleSerialPinChangedEvent(object sender, SerialPinChangedEventArgs e)
    {
        logger.LogInformation("Received serial pin change event: {Event}", e);
        if (e.EventType != SerialPinChange.CtsChanged) { return; }

        try
        {
            logger.LogError("The CTS pin has changed: {CtsHolding}", serialPort.CtsHolding);
        }
        catch (InvalidOperationException exception)
        {
            logger.LogWarning("The CTS pin has changed, but state could not be determined. {Event}, {Message}",e, exception.InnerException?.Message);
        }
    }

    public override bool SendMessage(byte[] messageBytes, int byteCount)
    {
        if (serialPort.IsOpen)
        {
            var message = new byte[byteCount];
            Array.Copy(messageBytes, 0, message, 0, byteCount);
            rawLogger.LogInformation(">>> {Data}", message.GetDataString());
            serialPort.Write(messageBytes, 0, byteCount);
        }
        else
        {
            logger.LogError("Cannot send message - Serial Port NOT connected or open");
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

        try
        {
            serialPort.PinChanged += HandleSerialPinChangedEvent;
            serialPort.DataReceived += HandleSerialPortDataReceived;
            serialPort.Open();
            serialPort.RtsEnable = true;

            var ctsHolding = serialPort.CtsHolding;
            logger.LogInformation("The current CTS pin: {ctsHolding}", ctsHolding);
            return new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialPort);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogError(ex, "Could not open serial port '{PortName}'", serialPort.PortName);
            return new ConnectionStateInfo(
                InterfaceConnectionState.Disconnected, 
                InterfaceConnectionType.SerialPort, 
                string.Format(CultureInfo.CurrentUICulture, Resources.Error_CouldNotConnectToSerialInterface, serialPort.PortName));
        }
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