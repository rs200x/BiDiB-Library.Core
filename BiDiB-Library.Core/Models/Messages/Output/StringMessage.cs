using org.bidib.netbidibc.core.Utils;
using System.Collections.Generic;

namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class StringMessage : BiDiBOutputMessage
{
    public StringMessage(byte[] address, byte @namespace, byte stringId, string value) : base(address, BiDiBMessage.MSG_STRING)
    {
        Namespace = @namespace;
        StringId = stringId;
        StringSize = (byte)value.Trim().Length;
        StringValue = value;

        var parameters = new List<byte> { Namespace, StringId };
        var stringBytes = value.Trim().GetBytesWithLength();
        parameters.AddRange(stringBytes);
        Parameters = parameters.ToArray();       
    }

    public string StringValue { get; }

    public byte StringSize { get; }

    public byte StringId { get; }

    public byte Namespace { get; }

    public override string ToString() => $"{base.ToString()}, NS: {Namespace}, Id: {StringId}, Size: {StringSize}, Value: {StringValue}";
}