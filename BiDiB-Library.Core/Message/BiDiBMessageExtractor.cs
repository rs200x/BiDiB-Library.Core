using System;
using System.Collections.Generic;
using System.Linq;
using org.bidib.netbidibc.core.Models.Messages;
using org.bidib.netbidibc.core.Models.Messages.Input;

namespace org.bidib.netbidibc.core.Message
{
    public class BiDiBMessageExtractor : IBiDiBMessageExtractor
    {
        private byte[] lastOutput;
        private const byte EscapeChar = 0xfd;
        private const byte MagicChar = 0xfe;

        public IEnumerable<BiDiBInputMessage> ExtractMessage(byte[] messageBytes, bool checkCrc)
        {
            List<BiDiBInputMessage> messages = new List<BiDiBInputMessage>();

            List<byte> outputBytes = new List<byte>();
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
                List<byte> messageBlockBytes = new List<byte>();
                foreach (byte b in outputBytes)
                {
                    if (b == MagicChar)
                    {
                        byte[] bytes = UnescapeMessage(messageBlockBytes.ToArray());
                        List<byte[]> subMessages = SplitMessages(bytes, true, out lastOutput).ToList();
                        messages.AddRange(subMessages.Select(MessageFactory.CreateInputMessage).Where(x => x != null));
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
                List<byte[]> subMessages = SplitMessages(outputBytes.ToArray(), false, out lastOutput).ToList();
                messages.AddRange(subMessages.Select(MessageFactory.CreateInputMessage).Where(x => x != null));
            }

            return messages;
        }


        private static byte[] UnescapeMessage(byte[] messageBytes)
        {
            List<byte> bytes = new List<byte>();
            bool escapeNext = false;

            foreach (byte messageByte in messageBytes)
            {
                if (messageByte == EscapeChar)
                {
                    escapeNext = true;
                }
                else
                {
                    byte currentByte = messageByte;
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
            List<byte[]> messages = new List<byte[]>();
            leftOvers = null;
            int index = 0;

            while (index < output.Length)
            {
                int messageSize = output[index] + 1;

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

                byte[] message = new byte[messageSize];
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
            int crc = 0;
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
}