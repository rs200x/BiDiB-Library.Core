using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Models.Messages.Output;
using org.bidib.netbidibc.core.Services.EventArgs;

namespace org.bidib.netbidibc.core
{
    public interface IBiDiBInterface : IDisposable
    {
        void Initialize();

        Task ConnectAsync(IConnectionConfig connectionConfig);

        Task DisconnectAsync();

        void RejectConnection();

        void LinkConnection();

        void Cleanup();

        void SendMessage(BiDiBMessage messageType, byte[] address, params byte[] parameters);

        void SendMessage(BiDiBOutputMessage message);

        TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage) where TResponseMessage : BiDiBInputMessage;

        void Register(IMessageReceiver messageReceiver);

        void Unregister(IMessageReceiver messageReceiver);

        IEnumerable<BiDiBNode> Nodes { get; }

        IEnumerable<BiDiBBoosterNode> Boosters { get; }

        ConnectionStateInfo ConnectionState { get; }

        event EventHandler<BiDiBNodeAddedEventArgs> NodeAdded;
        event EventHandler<BiDiBBoosterNodeAddedEventArgs> BoosterAdded;
        event EventHandler<BiDiBNodeRemovedEventArgs> NodeRemoved;
        event EventHandler<BiDiBBoosterNodeRemovedEventArgs> BoosterRemoved;
        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStatusChanged;

        void RefreshNodes();
    }
}