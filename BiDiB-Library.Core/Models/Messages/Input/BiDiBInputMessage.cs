using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

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

        var index = 0;
        Message = messageBytes;
        if (messageBytes.Length > 0 && messageBytes[0] == 0xFE)
        {
            index++; // skip first magic
            Size = 1;
        }
        Size += messageBytes[index++];

        IList<byte> addrBytes = [];

        while (messageBytes[index] != 0)
        {
            addrBytes.Add(messageBytes[index++]);
            if (index >= messageBytes.Length)
            {
                // handle if causing issues
            }
        }
        if (!addrBytes.Any())
        {
            addrBytes.Add(0);
        }

        Address = [.. addrBytes];
        index++;

        SequenceNumber = messageBytes[index++];
        MessageType = (BiDiBMessage)messageBytes[index++];

        // data
        IList<byte> dataBytes = [];

        while (index <= Size)
        {
            dataBytes.Add(messageBytes[index++]);
        }

        MessageParameters = [.. dataBytes];
    }

    public byte[] Message { get; }


    public byte[] Address { get; }

    public BiDiBMessage MessageType { get; }

    private byte Size { get; }

    public byte[] MessageParameters { get; }

    public byte SequenceNumber { get; }

    private void ValidateParameterCount(int expectedParams)
    {
        if (expectedParams > 0 && MessageParameters == null || MessageParameters.Length < expectedParams)
        {
            throw new InvalidDataException($"{MessageType} contains wrong data ({expectedParams}/{MessageParameters?.Length}) {MessageParameters.GetDataString()}");
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
        var addressString = string.Join(".", Address.Select(x => x.ToString(CultureInfo.CurrentCulture)));
        return $"{BiDiBConstants.InMessagePrefix} {MessageType,-25} addr:{addressString}, seq:{SequenceNumber}, size:{Size}";
    }
}