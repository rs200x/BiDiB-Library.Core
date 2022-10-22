using System;
using System.Collections.Generic;
using org.bidib.netbidibc.core.Models.Messages.Output;

namespace org.bidib.netbidibc.core.Message
{
    public interface IBiDiBMessageService : IDisposable
    {
        /// <summary>
        /// Sends a BiDiB message to the node
        /// </summary>
        /// <param name="messageType">the BiDiB message type</param>
        /// <param name="address">the node address</param>
        /// <param name="parameters">the message parameters</param>
        void SendMessage(BiDiBMessage messageType, byte[] address, params byte[] parameters);

        /// <summary>
        /// Sends a BiDiB message to the node
        /// </summary>
        /// <param name="outputMessage">the output message</param>
        void SendMessage(BiDiBOutputMessage outputMessage);

        /// <summary>
        /// Sends multiple BiDiB messages as bulk to the node
        /// </summary>
        /// <param name="outputMessages">the output messages</param>
        void SendMessages(ICollection<BiDiBOutputMessage> outputMessages);

        void ResetMessageSequenceNumber(byte[] address);

        void Activate();

        void Deactivate();

        void Cleanup();

        void ProcessMessage(byte[] messageBytes);
        
        void Register(IMessageReceiver messageReceiver);

        void Unregister(IMessageReceiver messageReceiver);
    }
}