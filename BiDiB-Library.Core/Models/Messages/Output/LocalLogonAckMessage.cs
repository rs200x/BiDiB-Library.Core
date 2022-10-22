using System.Collections.Generic;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class LocalLogonAckMessage : BiDiBOutputMessage
    {
        public LocalLogonAckMessage(byte[] uniqueId) : base(new byte[] { 0 }, BiDiBMessage.MSG_LOCAL_LOGON_ACK)
        {
            Node = 0;
            Uid = uniqueId;
            List<byte> parameters = new List<byte> { Node };
            parameters.AddRange(uniqueId);
            Parameters = parameters.ToArray();
        }

        public byte Node { get; }

        public byte[] Uid { get; }

        public override string ToString() => $"{base.ToString()}, Node: {Node}, UID: {Uid.GetDataString()}";
    }
}