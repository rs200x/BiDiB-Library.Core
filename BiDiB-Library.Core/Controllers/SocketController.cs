using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Properties;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Controllers
{
    public class SocketController : ConnectionController<INetConfig>, ISocketController
    {
        private readonly ILogger<SocketController> logger;
        private readonly ILogger rawLogger;

        private byte[] clientData = new byte[512];
        private DnsEndPoint dnsEndPoint;
        private Socket senderSocket;
        private const int BufferSize = 8 * 1024;

        public SocketController(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<SocketController>();
            rawLogger = loggerFactory.CreateLogger("Raw");
        }

        protected bool IsConnected => senderSocket != null && senderSocket.Connected;

        public override ConnectionStateInfo ConnectionState => senderSocket != null && senderSocket.Connected
            ? new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialOverTcp)
            : new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialOverTcp);

        public override string ConnectionName => IsConnected ? senderSocket.RemoteEndPoint.ToString() : $"{dnsEndPoint.Host}:{dnsEndPoint.Port}";

        public override void Initialize(INetConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            try
            {
                dnsEndPoint = new DnsEndPoint(config.NetworkHostAddress, config.NetworkPortNumber);
                senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception e) when (e is SocketException || e is ArgumentNullException || e is ArgumentException)
            {
                logger.LogError(e, $"Problem creating socket on {config.NetworkHostAddress}:{config.NetworkPortNumber}");
                throw;
            }
        }

        public override bool SendMessage(byte[] messageBytes, int byteCount)
        {
            if (!senderSocket.Connected)
            {
                logger.LogError($"Cannot send message - Socket to '{ConnectionName}' is NOT connected or open!");
                return false;
            }

            try
            {
                byte[] msgToSend = EncodeMessage(messageBytes, byteCount);
                rawLogger.LogInformation($">>> {messageBytes.GetDataString()}");
                logger.LogInformation($">>> {messageBytes.GetDataString()}");
                senderSocket.Send(msgToSend);
            }
            catch (Exception e) when (e is SocketException)
            {
                logger.LogError(e, $"Sending data to '{ConnectionName}' failed");
            }

            return true;
        }

        internal virtual byte[] EncodeMessage(byte[] messageBytes, int byteCount)
        {
            return messageBytes;
        }

        public override Task<ConnectionStateInfo> OpenConnectionAsync()
        {
            return Task.Factory.StartNew(ConnectToSocket);
        }

        private ConnectionStateInfo ConnectToSocket()
        {
            string error = string.Empty;
            InterfaceConnectionState interfaceState;
            try
            {
                senderSocket.Connect(dnsEndPoint);
                StartReceive();
                interfaceState = InterfaceConnectionState.FullyConnected;
                Task.Run(ObserveConnection);
            }
            catch (Exception e) when (e is SocketException)
            {
                error = string.Format(CultureInfo.CurrentUICulture, Resources.Error_CouldNotConnectToSocket, dnsEndPoint);
                logger.LogError(e, error);
                interfaceState = InterfaceConnectionState.Disconnected;
            }

            return new ConnectionStateInfo(interfaceState, InterfaceConnectionType.SerialOverTcp, error);
        }

        private void StartReceive()
        {
            clientData = new byte[BufferSize];

            if (senderSocket.Poll(0, SelectMode.SelectRead) && senderSocket.Available == 0)
            {
                logger.LogWarning($"Socket '{ConnectionName}' seems to be closed");
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
                logger.LogError(se, Resources.Error_SocketData);
            }
        }

        private async Task ObserveConnection()
        {
            while(senderSocket != null && senderSocket.Connected)
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

                if (!(result.AsyncState is Socket socket))
                {
                    logger.LogError(Resources.Error_NoSocket);
                    return;
                }

                // get the actual message and fill out the source:
                int messageSize = socket.EndReceive(result);
                // do what you'd like with `message` here:
                logger.LogDebug($"Socket data received {BitConverter.ToString(clientData, 0, messageSize)}");

                if (messageSize > 0)
                {
                    ProcessMessage(clientData, messageSize);
                }

                // schedule the next receive operation once reading is done:
                StartReceive();
            }
            catch (ObjectDisposedException)
            {
                logger.LogDebug(Resources.Error_SocketDisposed);
            }

            catch (SocketException se)
            {
                logger.LogError(se, Resources.Error_SocketData);
            }
        }

        protected virtual byte[] DecodeMessage(byte[] message, int byteCount)
        {
            byte[] messageData = new byte[byteCount];
            Array.Copy(message, 0, messageData, 0, byteCount);
            return messageData;
        }

        public virtual void ProcessMessage(byte[] message, int messageSize)
        {
            byte[] messageData = DecodeMessage(message, messageSize);
            rawLogger.LogInformation($"<<< {messageData.GetDataString()}");
            logger.LogInformation($"<<< {messageData.GetDataString()}");
            ProcessReceivedData?.Invoke(messageData);
        }

        public override void Close()
        {
            if (senderSocket.Connected)
            {
                senderSocket.Close(0);
            }
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
}