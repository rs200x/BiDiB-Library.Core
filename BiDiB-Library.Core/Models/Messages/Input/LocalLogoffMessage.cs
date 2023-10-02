using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_LOCAL_LOGOFF)]
public class LocalLogoffMessage : BiDiBInputMessage
{
    public LocalLogoffMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_LOGOFF, 7)
    {
        Uid = MessageParameters;
    }

    public byte[] Uid { get; }

    public override string ToString() => $"{base.ToString()}, UID: {Uid.GetDataString()}";
}