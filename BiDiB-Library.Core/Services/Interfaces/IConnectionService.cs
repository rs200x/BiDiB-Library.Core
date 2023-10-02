using System;
using System.Threading.Tasks;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Services.EventArgs;

namespace org.bidib.Net.Core.Services.Interfaces;

public interface IConnectionService : IDisposable
{
    void ConfigureConnection(IConnectionConfig connectionConfig);

    Task<ConnectionStateInfo> OpenConnectionAsync();

    void CloseConnection();

    void RejectConnection();

    void LinkConnection();

    void SendData(byte[] messageBytes, int messageSize);
        
    ConnectionStateInfo ConnectionState { get; }

    bool MessageSecurity { get; }

    event Action<byte[]> DataReceived;

    event EventHandler<System.EventArgs> ConnectionClosed;

    event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

    string GetConnectionName();
}