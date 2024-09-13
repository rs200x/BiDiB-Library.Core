using System.Collections.Generic;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.BiDiB.Base;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Models.Messages.Output;

namespace org.bidib.Net.Core.Message;

public interface IBiDiBMessageProcessor
{
    /// <summary>
    /// Queries for all child nodes of the specified parent address
    /// </summary>
    /// <param name="parentAddress">The parent node address</param>
    void GetChildNodes(byte[] parentAddress);

    TResponseMessage SendMessage<TResponseMessage>(BiDiBNode node, BiDiBMessage messageType, int timeout, bool acceptFromAnySender, params byte[] parameters) where TResponseMessage : BiDiBInputMessage;
    
    TResponseMessage SendMessage<TResponseMessage>(BiDiBNode node, BiDiBMessage messageType, int timeout, params byte[] parameters) where TResponseMessage : BiDiBInputMessage;

    TResponseMessage SendMessage<TResponseMessage>(BiDiBNode node, BiDiBMessage messageType, params byte[] parameters) where TResponseMessage : BiDiBInputMessage;

    TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage) where TResponseMessage : BiDiBInputMessage;

    TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage, int timeout, bool acceptFromAnySender) where TResponseMessage : BiDiBInputMessage;
    
    TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage, int timeout) where TResponseMessage : BiDiBInputMessage;

    void SendMessage(BiDiBOutputMessage outputMessage);

    void SendMessage(BiDiBNode node, BiDiBMessage messageType, params byte[] parameters);

    IEnumerable<Port> GetPorts(BiDiBNode node, PortType portType, int expectedItems);
        
    IEnumerable<TResponseMessage> SendMessages<TResponseMessage>(ICollection<BiDiBOutputMessage> outputMessages, int timeout = 300) where TResponseMessage : BiDiBInputMessage;
}