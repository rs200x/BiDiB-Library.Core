﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Properties;
using org.bidib.netbidibc.core.Services.EventArgs;
using org.bidib.netbidibc.core.Services.Interfaces;

namespace org.bidib.netbidibc.core.Services
{

    internal sealed class ConnectionService : IConnectionService
    {
        private readonly ILogger<ConnectionService> logger;
        private IConnectionController activeConnectionController;
        private readonly IEnumerable<IConnectionControllerFactory> connectionControllerFactories;

        public ConnectionService(IEnumerable<IConnectionControllerFactory> connectionControllerFactories, ILogger<ConnectionService> logger)
        {
            this.logger = logger;
            this.connectionControllerFactories = connectionControllerFactories;
        }

        public void ConfigureConnection(IConnectionConfig connectionConfig)
        {
            logger.LogDebug($"initialize with {connectionConfig.ConnectionType}");
            MessageSecurity = true;

            var controllerFactory = connectionControllerFactories.FirstOrDefault(x => x.ConnectionType == connectionConfig.ConnectionType);
            if (controllerFactory == null)
            {
                throw new InvalidOperationException($"There is no factory provided for connection type {connectionConfig.ConnectionType}");
            }

            activeConnectionController = controllerFactory.GetController(connectionConfig);
            if (activeConnectionController == null)
            {
                throw new InvalidOperationException($"There is no controller for connection type {connectionConfig.ConnectionType}");
            }

            BiDiBMessageGenerator.SecureMessages = activeConnectionController.MessageSecurityEnabled;
            MessageSecurity = activeConnectionController.MessageSecurityEnabled;

            activeConnectionController.ProcessReceivedData = OnDataReceived;
            activeConnectionController.ConnectionClosed += HandleControllerConnectionClosed;
            activeConnectionController.ConnectionStateChanged += HandleControllerConnectionStateChanged;
        }

        public async Task<ConnectionStateInfo> OpenConnectionAsync()
        {
            if (activeConnectionController == null) { throw new InvalidOperationException("connection service is not configured for any connection"); }

            return await activeConnectionController.OpenConnectionAsync().ConfigureAwait(false);
        }

        public void CloseConnection()
        {
            if (activeConnectionController == null)
            {
                logger.LogError(Resources.Error_NotConfiguredForConnection);
                return;
            }

            activeConnectionController.Close();
        }

        public void RejectConnection()
        {
            activeConnectionController?.RejectControl();
        }

        public void LinkConnection()
        {
            activeConnectionController?.RequestControl();
        }

        public void SendData(byte[] messageBytes, int messageSize)
        {
            activeConnectionController.SendMessage(messageBytes, messageSize);
        }

        public ConnectionStateInfo ConnectionState => activeConnectionController != null
            ? activeConnectionController.ConnectionState
            : new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.SerialPort);

        public bool MessageSecurity { get; private set; }

        public event Action<byte[]> DataReceived;

        public event EventHandler<System.EventArgs> ConnectionClosed;

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public string GetConnectionName()
        {
            return activeConnectionController.ConnectionName;
        }

        public void Dispose()
        {
            if (activeConnectionController == null) { return; }

            activeConnectionController.ConnectionStateChanged -= HandleControllerConnectionStateChanged;
            activeConnectionController.ConnectionClosed -= HandleControllerConnectionClosed;
        }

        private void OnDataReceived(byte[] dataBytes) => DataReceived?.Invoke(dataBytes);

        private void HandleControllerConnectionClosed(object sender, System.EventArgs eventArgs)
        {
            activeConnectionController.ConnectionClosed -= HandleControllerConnectionClosed;
            ConnectionClosed?.Invoke(this, System.EventArgs.Empty);
        }

        private void HandleControllerConnectionStateChanged(object sender, System.EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(ConnectionState));
        }
    }
}