using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Properties;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Controllers;


public class SocketController : SocketController<INetConfig>
{
    public SocketController(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
}

[Localizable(false)]
public class SocketController<TConfig> : ConnectionController<TConfig>, ISocketController<TConfig>
    where TConfig : INetConfig
{
    private readonly ILogger<SocketController> logger;
    private readonly ILogger rawLogger;

    private byte[] clientData = new byte[512];
    private DnsEndPoint dnsEndPoint;
    private Socket senderSocket;
    private const int BufferSize = 8 * 1024;

    protected SocketController(ILoggerFactory loggerFactory)
    {
        if (loggerFactory == null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        logger = loggerFactory.CreateLogger<SocketController>();
        rawLogger = loggerFactory.CreateLogger(BiDiBConstants.LoggerContextRaw);
    }

    protected bool IsConnected => senderSocket is { Connected: true };

    public override ConnectionStateInfo ConnectionState => senderSocket is { Connected: true }
        ? new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialOverTcp)
        : new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialOverTcp);

    public override string ConnectionName => IsConnected ? senderSocket.RemoteEndPoint?.ToString() : $"{dnsEndPoint.Host}:{dnsEndPoint.Port}";
    
    public override void Initialize(TConfig config)
    {
        if (Equals(config, default(TConfig)))
        {
            throw new ArgumentNullException(nameof(config));
        }

        try
        {
            dnsEndPoint = new DnsEndPoint(config.NetworkHostAddress, config.NetworkPortNumber);
            senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        catch (Exception e) when (e is SocketException or ArgumentNullException or ArgumentException)
        {
            logger.LogError(e, "Problem creating socket on {Address}:{PortNumber}", config.NetworkHostAddress, config.NetworkPortNumber);
            throw;
        }
    }


    public override bool SendMessage(byte[] messageBytes, int byteCount)
    {
        if (!senderSocket.Connected)
        {
            logger.LogError("Cannot send message - Socket to '{ConnectionName}' is NOT connected or open!", ConnectionName);
            return false;
        }

        try
        {
            var msgToSend = EncodeMessage(messageBytes, byteCount);
            var outMessage = messageBytes.GetDataString();
            rawLogger.LogInformation(">>> {Data}", outMessage);
            logger.LogInformation(">>> {Data}", outMessage);
            senderSocket.Send(msgToSend);
        }
        catch (Exception e) when (e is SocketException)
        {
            logger.LogError(e, "Sending data to '{ConnectionName}' failed", ConnectionName);
        }

        return true;
    }

    protected virtual byte[] EncodeMessage(byte[] messageBytes, int byteCount)
    {
        return messageBytes;
    }

    public override Task<ConnectionStateInfo> OpenConnectionAsync()
    {
        return Task.Factory.StartNew(ConnectToSocket);
    }

    private ConnectionStateInfo ConnectToSocket()
    {
        try
        {
            senderSocket.Connect(dnsEndPoint);
            StartReceive();
            Task.Run(ObserveConnectionAsync);
            return new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialOverTcp);
        }
        catch (Exception e) when (e is SocketException)
        {
            logger.LogError(e, "Could not establish interface connection to endpoint '{EndPoint}'", dnsEndPoint);
            return new ConnectionStateInfo(
                InterfaceConnectionState.Disconnected,
                InterfaceConnectionType.SerialOverTcp,
                string.Format(CultureInfo.CurrentUICulture, Resources.Error_CouldNotConnectToSocket, dnsEndPoint));
        }
    }

    private void StartReceive()
    {
        clientData = new byte[BufferSize];

        if (senderSocket.Poll(0, SelectMode.SelectRead) && senderSocket.Available == 0)
        {
            logger.LogWarning("Socket '{ConnectionName}' seems to be closed", ConnectionName);
            Close();

            OnConnectionClosed();
            return;
        }

        try
        {
            senderSocket.BeginReceive(clientData, 0, clientData.Length, SocketFlags.None, HandleDataReceived, senderSocket);
        }
        catch (SocketException se)
        {
            logger.LogError(se, "Error when handling received data");
        }
    }

    private async Task ObserveConnectionAsync()
    {
        while (senderSocket is { Connected: true })
        {
            await Task.Delay(200);
        }

        logger.LogInformation("Socket has been closed");
    }

    private void HandleDataReceived(IAsyncResult result)
    {
        try
        {
            // this is what had been passed into BeginReceive as the second parameter:

            if (result.AsyncState is not Socket socket)
            {
                logger.LogError("Data result is not a socket!");
                return;
            }

            // get the actual message and fill out the source:
            var messageSize = socket.EndReceive(result);
            // do what you'd like with `message` here:
            logger.LogDebug("Socket data received {Data}", BitConverter.ToString(clientData, 0, messageSize));

            if (messageSize > 0)
            {
                ProcessMessage(clientData, messageSize);
            }

            // schedule the next receive operation once reading is done:
            StartReceive();
        }
        catch (ObjectDisposedException)
        {
            logger.LogDebug("Socket has been disposed!");
        }

        catch (SocketException se)
        {
            logger.LogError(se, "Error when handling received data");
        }
    }

    protected virtual byte[] DecodeMessage(byte[] message, int byteCount)
    {
        var messageData = new byte[byteCount];
        Array.Copy(message, 0, messageData, 0, byteCount);
        return messageData;
    }

    public virtual void ProcessMessage(byte[] message, int messageSize)
    {
        var messageBytes = DecodeMessage(message, messageSize);
        var messageData = messageBytes.GetDataString();
        rawLogger.LogInformation("<<< {Data}", messageData);
        logger.LogInformation("<<< {Data}", messageData);
        ProcessReceivedData?.Invoke(messageBytes);
    }

    public override void Close()
    {
        if (senderSocket is not { Connected: true })
        {
            return;
        }

        senderSocket.Shutdown(SocketShutdown.Both);
        senderSocket.Close(0);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) { return; }

        senderSocket?.Dispose();
    }
}