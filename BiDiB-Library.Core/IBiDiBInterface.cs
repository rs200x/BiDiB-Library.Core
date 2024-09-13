using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Models.Messages.Output;
using org.bidib.Net.Core.Services.EventArgs;

namespace org.bidib.Net.Core;

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

    /// <summary>
    /// Sends a bidib message to the bus
    /// </summary>
    /// <typeparam name="TResponseMessage">Type of the expected response message</typeparam>
    /// <param name="outputMessage">The message to be sent</param>
    /// <param name="timeout">Timeout in ms to wait for response</param>
    /// <param name="acceptFromAnySender">Defines if message matching response type is accepted from any node</param>
    /// <returns>The expected message object</returns>
    TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage, int timeout = 500, bool acceptFromAnySender = false) where TResponseMessage : BiDiBInputMessage;

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