namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_STRING_GET)]
public class StringGetMessage : BiDiBInputMessage
{
    public StringGetMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_STRING_GET, 2)
    {
        Namespace = MessageParameters[0];
        StringId = MessageParameters[1];
    }

    public byte StringId { get; }

    public byte Namespace { get; }

    public override string ToString() => $"{base.ToString()}, NS: {Namespace}, Id: {StringId}";
}