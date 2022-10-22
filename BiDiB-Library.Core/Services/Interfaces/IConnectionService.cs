using System;
using System.Threading.Tasks;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Services.EventArgs;

namespace org.bidib.netbidibc.core.Services.Interfaces
{
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
}