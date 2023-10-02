using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Utils;

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
    
    public static string GetSoftwareVersionString(this byte[] versionBytes)
    {
        return versionBytes is not {Length: 3} 
            ? string.Empty 
            : $"{versionBytes[0]:d}.{versionBytes[1]:d2}.{versionBytes[2]:D2}";
    }

    public static int CalculateBoosterCurrent(this byte current)
    {
        if (current == 0)
        {
            return 0;
        }
        
        var calculateCurrent = current switch
        {
            < 16 => current,
            < 64 => (current - 12) * 4,
            < 128 => (current - 51) * 16,
            < 192 => (current - 108) * 64,
            < 251 => (current - 171) * 256,
            254 => -2,
            _ => -1
        };

        return calculateCurrent;
    }

    public static ushort GetDecoderAddress(byte low, byte high)
    {
        var addrHigh = Convert.ToByte(high & 0x3f);
        return BitConverter.ToUInt16(new[] { low, addrHigh }, 0);
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

        var allSplit = false;
        do
        {
            if (bytes == null || bytes.Length == 0)
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