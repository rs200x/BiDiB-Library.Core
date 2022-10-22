using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    [InputMessage(BiDiBMessage.MSG_LOCAL_LOGON)]
    public class LocalLogonMessage : BiDiBInputMessage
    {
        internal LocalLogonMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_LOGON, 7)
        {
            Uid = MessageParameters;
        }

        public byte[] Uid { get; }

        public override string ToString() => $"{base.ToString()}, UID: {Uid.GetDataString()}";
    }
}