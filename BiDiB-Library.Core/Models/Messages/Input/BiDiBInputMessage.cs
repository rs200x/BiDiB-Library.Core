using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class BiDiBInputMessage
    {
        internal BiDiBInputMessage(byte[] messageBytes, BiDiBMessage expectedMessageType, int expectedParameters) : this(messageBytes)
        {
            ValidateMessageType(expectedMessageType);
            ValidateParameterCount(expectedParameters);
        }

        internal BiDiBInputMessage(byte[] messageBytes)
        {
            if (messageBytes == null || messageBytes.Length < 1) { throw new ArgumentNullException(nameof(messageBytes)); }

            int index = 0;
            Message = messageBytes;
            if (messageBytes.Length > 0 && messageBytes[0] == 0xFE)
            {
                index++; // skip first magic
                Size = 1;
            }
            Size += messageBytes[index++];

            IList<byte> addrBytes = new List<byte>();

            while (messageBytes[index] != 0)
            {
                addrBytes.Add(messageBytes[index++]);
                if (index >= messageBytes.Length)
                {
                    //logger.LogWarning("Invalid message: {}", messageBytes);
                    //throw new ProtocolException("address too long");
                }
            }
            if (!addrBytes.Any())
            {
                addrBytes.Add(0);
            }

            Address = addrBytes.ToArray();
            index++;

            SequenceNumber = messageBytes[index++];
            MessageType = (BiDiBMessage)messageBytes[index++];

            // data
            IList<byte> dataBytes = new List<byte>();

            while (index <= Size)
            {
                dataBytes.Add(messageBytes[index++]);
            }

            MessageParameters = dataBytes.ToArray();
        }

        public byte[] Message { get; }


        public byte[] Address { get; }

        public BiDiBMessage MessageType { get; }

        public byte Size { get; }

        public byte[] MessageParameters { get; }

        public byte SequenceNumber { get; }

        private void ValidateParameterCount(int expectedParams)
        {
            if (expectedParams > 0 && MessageParameters == null || MessageParameters.Length < expectedParams)
            {
                throw new InvalidDataException($"{MessageType} contains wrong data {MessageParameters.GetDataString()}");
            }
        }

        private void ValidateMessageType(BiDiBMessage expectedMessageType)
        {
            if (MessageType != expectedMessageType)
            {
                throw new InvalidDataException($"The message type {MessageType} does not match the expected {expectedMessageType} type");
            }
        }

        public override string ToString()
        {
            string addressString = string.Join(".", Address.Select(x => x.ToString(CultureInfo.CurrentCulture)));
            return $"{BiDiBConstants.InMessagePrefix} {MessageType,-25} addr:{addressString}, seq:{SequenceNumber}, size:{Size}";
        }
    }
}