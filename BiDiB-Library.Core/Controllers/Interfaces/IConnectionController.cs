using System;
using System.Threading.Tasks;
using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Controllers.Interfaces;

public interface IConnectionController
{
    /// <summary>
    /// Send a message via the serial port
    /// </summary>
    /// <param name="messageBytes"></param>
    /// <param name="byteCount"></param>
    /// <returns><c>true</c> if message was send; otherwise <c>false</c></returns>
    bool SendMessage(byte[] messageBytes, int byteCount);

    Task<ConnectionStateInfo> OpenConnectionAsync();

    void Close();

    Action<byte[]> ProcessReceivedData { get; set; }

    /// <summary>
    /// Event raised when connection gets closed
    /// </summary>
    event EventHandler<EventArgs> ConnectionClosed;

    /// <summary>
    /// Determines the connection state to the interface
    /// </summary>
    ConnectionStateInfo ConnectionState { get; }

    /// <summary>
    /// Event raised when connection state has changed
    /// </summary>
    event EventHandler<EventArgs> ConnectionStateChanged;

    /// <summary>
    /// Determines whether message security is required for the used connection type
    /// </summary>
    bool MessageSecurityEnabled { get; }

    /// <summary>
    /// Determines the connection name
    /// </summary>
    string ConnectionName { get; }

    /// <summary>
    /// Request the control of the bidib system
    /// </summary>
    void RequestControl();

    /// <summary>
    /// Reject the control of the bidib system
    /// </summary>
    void RejectControl();
}

public interface IConnectionController<in TConfig> : IConnectionController
    where TConfig : IConnectionConfig
{
    void Initialize(TConfig config);
}