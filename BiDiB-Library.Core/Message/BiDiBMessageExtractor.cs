using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using org.bidib.Net.Core.Models.Messages.Input;

namespace org.bidib.Net.Core.Message;

[Export(typeof(IBiDiBMessageExtractor))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class BiDiBMessageExtractor : IBiDiBMessageExtractor
{
    private readonly IMessageFactory messageFactory;
    private byte[] lastOutput;
    private const byte EscapeChar = 0xfd;
    private const byte MagicChar = 0xfe;

    [ImportingConstructor]
    public BiDiBMessageExtractor(IMessageFactory messageFactory)
    {
        this.messageFactory = messageFactory;
    }

    public IEnumerable<BiDiBInputMessage> ExtractMessage(byte[] messageBytes, bool checkCrc)
    {
        var messages = new List<BiDiBInputMessage>();

        var outputBytes = new List<byte>();
        if (lastOutput != null)
        {
            outputBytes.AddRange(lastOutput);
            lastOutput = null;
        }

        outputBytes.AddRange(messageBytes);

        // detect and remove starting Magic
        if (outputBytes.Count > 0 && outputBytes[0] == MagicChar)
        {
            outputBytes.RemoveAt(0);
        }

        if (checkCrc)
        {
            var messageBlockBytes = new List<byte>();
            foreach (var b in outputBytes)
            {
                if (b == MagicChar)
                {
                    var bytes = RemoveMagicEscapesFromMessage(messageBlockBytes.ToArray());
                    var subMessages = SplitMessages(bytes, true, out lastOutput).ToList();
                    messages.AddRange(subMessages.Select(messageFactory.CreateInputMessage).Where(x => x != null));
                    messageBlockBytes.Clear();
                }
                else
                {
                    messageBlockBytes.Add(b);
                }
            }

            lastOutput = messageBlockBytes.ToArray();
        }
        else
        {
            var subMessages = SplitMessages(outputBytes.ToArray(), false, out lastOutput).ToList();
            messages.AddRange(subMessages.Select(messageFactory.CreateInputMessage).Where(x => x != null));
        }

        return messages;
    }


    private static byte[] RemoveMagicEscapesFromMessage(byte[] messageBytes)
    {
        var bytes = new List<byte>();
        var escapeNext = false;

        foreach (var messageByte in messageBytes)
        {
            if (messageByte == EscapeChar)
            {
                escapeNext = true;
            }
            else
            {
                var currentByte = messageByte;
                if (escapeNext)
                {
                    currentByte = Convert.ToByte(currentByte ^ 0x20);
                    escapeNext = false;
                }

                bytes.Add(currentByte);
            }
        }

        return bytes.ToArray();
    }

    private static IEnumerable<byte[]> SplitMessages(byte[] output, bool checkCrc, out byte[] leftOvers)
    {
        var messages = new List<byte[]>();
        leftOvers = null;
        var index = 0;

        while (index < output.Length)
        {
            var messageSize = output[index] + 1;

            if (messageSize <= 0)
            {
                throw new InvalidOperationException("cannot split messages, array size is " + messageSize);
            }

            if (messageSize > output.Length - index)
            {
                leftOvers = new byte[output.Length - index];
                Array.Copy(output, index, leftOvers, 0, leftOvers.Length);
                break;
            }

            var message = new byte[messageSize];
            Array.Copy(output, index, message, 0, message.Length);
            index += messageSize;

            messages.Add(message);

            if (!checkCrc || index != output.Length - 1) continue;

            ValidateCrc(output);
            break;
        }

        return messages;
    }

    private static void ValidateCrc(IList<byte> message)
    {
        var crc = 0;
        int index;

        for (index = 0; index < message.Count - 1; index++)
        {
            crc = BiDiBMessageGenerator.CrcBytes[(message[index] ^ crc) & 0xFF];
        }
        if (crc != (message[index] & 0xFF))
        {
            throw new InvalidOperationException("CRC failed: should be " + crc + " but was " + (message[index] & 0xFF));
        }
    }
}