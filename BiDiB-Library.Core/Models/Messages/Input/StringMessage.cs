using System;
using System.Text;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_STRING)]
public class StringMessage : BiDiBInputMessage
{
    public StringMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_STRING, 3)
    {
        Namespace = MessageParameters[0];
        StringId = MessageParameters[1];
        StringSize = MessageParameters[2];

        StringBuilder sb = new();

        for (var i = 3; i < StringSize + 3; i++)
        {
            sb.Append(Convert.ToChar(MessageParameters[i]));
        }

        StringValue = sb.ToString();
    }

    public string StringValue { get; }

    public byte StringSize { get; }

    public byte StringId { get; }

    public byte Namespace { get; }

    public override string ToString() => $"{base.ToString()}, NS: {Namespace}, Id: {StringId}, Size: {StringSize}, Value: {StringValue}";
}