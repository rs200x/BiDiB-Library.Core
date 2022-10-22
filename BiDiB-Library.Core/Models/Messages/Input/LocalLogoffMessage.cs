using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class LocalLogoffMessage : BiDiBInputMessage
    {
        internal LocalLogoffMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_LOGOFF, 7)
        {
            Uid = MessageParameters;
        }

        public byte[] Uid { get; }

        public override string ToString() => $"{base.ToString()}, UID: {Uid.GetDataString()}";
    }
}