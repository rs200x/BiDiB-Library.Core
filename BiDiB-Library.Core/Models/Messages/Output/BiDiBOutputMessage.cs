using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class BiDiBOutputMessage
    {
        public BiDiBOutputMessage(byte[] address, BiDiBMessage messageType)
            : this(address, messageType, Array.Empty<byte>())
        {
        }

        public BiDiBOutputMessage(byte[] address, BiDiBMessage messageType, byte[] parameters) 
            : this(address, (byte)messageType, parameters)
        {
        }

        public BiDiBOutputMessage(byte[] address, byte messageType, byte[] parameters)
        {
            Address = address;
            MessageTypeByte = messageType;
            Parameters = parameters;
            SequenceNumber = 0;

            MessageType = Enum.IsDefined(typeof(BiDiBMessage), messageType)
                ? (BiDiBMessage) messageType
                : BiDiBMessage.Undefined;
        }

        public byte[] Address { get; }

        public BiDiBMessage MessageType { get; }

        public byte MessageTypeByte { get; }

        public byte SequenceNumber { get; set; }

        public byte[] Parameters { get; protected set; }

        public byte[] GetMessageBytes()
        {
            List<byte> messageBytes = new List<byte>();

            // local addr
            messageBytes.AddRange(Address);
            if (Address.Last() > 0)
            {
                messageBytes.Add(0); // last address byte is always 0
            }

            // message number
            messageBytes.Add(SequenceNumber);

            // message id
            messageBytes.Add(MessageTypeByte);

            // add parameters
            messageBytes.AddRange(Parameters);

            // message size
            messageBytes.Insert(0, Convert.ToByte(messageBytes.Count));

            return messageBytes.ToArray();
        }

        public override string ToString()
        {
            string addressString = string.Join(".", Address.Select(x => x.ToString(CultureInfo.CurrentCulture)));
            return $"{BiDiBConstants.OutMessagePrefix} {MessageType,-25} addr:{addressString}, seq:{SequenceNumber}";
        }
    }
}