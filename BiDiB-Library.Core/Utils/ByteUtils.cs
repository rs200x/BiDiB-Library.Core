using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Utils
{
    public static class ByteUtils
    {
        public static int GetArrayValue(this byte[] bytes) => bytes?.Select((t, index) => t << (index * 8)).Sum() ?? 0;

        public static string GetStringValue(this byte[] bytes) => bytes.Aggregate("", (current, b) => current + Convert.ToChar(b));

        public static string GetDataString(this byte[] bytes) => bytes != null ? BitConverter.ToString(bytes, 0, bytes.Length) : "-";

        public static string GetBitString(this byte @byte)
        {
            var array = new BitArray(new[] { @byte });
            return string.Join(string.Empty, array.OfType<bool>().Select(x => x ? '1' : '0'));
        }

        public static int CalculateBoosterCurrent(this byte cur)
        {
            var ccur = 0;
            if (cur == 0)
            {
                return ccur;
            }
            ccur = -1;
            if (cur < 16)
            {
                ccur = cur;
            }
            else if (cur < 64)
            {
                ccur = (cur - 12) * 4;
            }
            else if (cur < 128)
            {
                ccur = (cur - 51) * 16;
            }
            else if (cur < 192)
            {
                ccur = (cur - 108) * 64;
            }
            else if (cur < 251)
            {
                ccur = (cur - 171) * 256;
            }
            else if (cur == 254)
            {
                ccur = -2;
            }

            return ccur;
        }

        public static ushort GetDecoderAddress(byte low, byte high)
        {
            byte addrLow = low;
            byte addrHigh = Convert.ToByte(high & 0x3f);
            return BitConverter.ToUInt16(new[] { addrLow, addrHigh }, 0);
        }

        public static DecoderDirection GetDecoderDirection(this byte addressHigh)
        {
            return (DecoderDirection)((addressHigh & 0xc0) >> 6);
        }

        public static byte[] GetBytes(this string value)
        {
            return value.Select(Convert.ToByte).ToArray();
        }

        public static byte[] GetBytesWithLength(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new byte[] { 0 };
            }

            List<byte> parameters = new() { Convert.ToByte(value.Length) };
            parameters.AddRange(value.Select(Convert.ToByte));
            return parameters.ToArray();
        }

        public static byte[] GetBytesFromDataString(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<byte>();
            }

            var parameters = value.Split('-');
            var bytes = new List<byte>();
            foreach (var item in parameters)
            {
                var byteValue = Convert.ToByte(item, 16);
                bytes.Add(byteValue);
            }

            return bytes.ToArray();
        }

        public static IEnumerable<byte[]> SplitByFirst(this byte[] bytes)
        {
            List<byte[]> messages = new();

            bool allSplit = false;
            do
            {
                if (bytes.Length == 0)
                {
                    allSplit = true;
                    continue;
                }

                if (bytes[0] == 254)
                {
                    bytes = bytes.Skip(1).ToArray();
                }

                var size = bytes[0] + 1;
                var subMessage = new byte[size];
                Array.Copy(bytes, 0, subMessage, 0, size);
                messages.Add(subMessage);
                bytes = bytes.Skip(size).ToArray();

                if (bytes.Length == 0 || bytes[0] > bytes.Length)
                {
                    allSplit = true;
                }

            } while (!allSplit);

            return messages;
        }
    }
}