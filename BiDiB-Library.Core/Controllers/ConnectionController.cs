using System;
using System.Threading.Tasks;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Controllers;

public abstract class ConnectionController : IConnectionController
{
    public Action<byte[]> ProcessReceivedData { get; set; }

    /// <inheritdoc />
    public event EventHandler<EventArgs> ConnectionClosed;

    /// <inheritdoc />
    public abstract ConnectionStateInfo ConnectionState { get; }

    /// <inheritdoc />
    public event EventHandler<EventArgs> ConnectionStateChanged;

    /// <inheritdoc />
    public virtual bool MessageSecurityEnabled => true;

    /// <inheritdoc />
    public abstract string ConnectionName { get; }

    /// <inheritdoc />
    public virtual void RequestControl()
    {
        // nothing to do
    }

    /// <inheritdoc />
    public virtual void RejectControl()
    {
        // nothing to do
    }

    /// <inheritdoc />
    public abstract bool SendMessage(byte[] messageBytes, int byteCount);

    /// <inheritdoc />
    public abstract Task<ConnectionStateInfo> OpenConnectionAsync();

    /// <inheritdoc />
    public abstract void Close();

    protected void OnConnectionClosed()
    {
        ConnectionClosed?.Invoke(this, EventArgs.Empty);
    }

    protected void OnConnectionStateChanged()
    {
        ConnectionStateChanged?.Invoke(this, EventArgs.Empty);
    }
}

public abstract class ConnectionController<TConfig> : ConnectionController, IConnectionController<TConfig>
    where TConfig : IConnectionConfig

{
    public abstract void Initialize(TConfig config);
}