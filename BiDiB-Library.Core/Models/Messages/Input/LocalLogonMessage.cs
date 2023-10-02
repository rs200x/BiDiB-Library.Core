using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_LOCAL_LOGON)]
public class LocalLogonMessage : BiDiBInputMessage
{
    public LocalLogonMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_LOGON, 7)
    {
        Uid = MessageParameters;
    }

    public byte[] Uid { get; }

    public override string ToString() => $"{base.ToString()}, UID: {Uid.GetDataString()}";
}