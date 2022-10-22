using org.bidib.netbidibc.core.Models.BiDiB.Extensions;
using org.bidib.netbidibc.core.Utils;
using System;
using System.Linq;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    [InputMessage(BiDiBMessage.MSG_LOCAL_LOGON_ACK)]
    public class LocalLogonAckMessage : BiDiBInputMessage
    {
        public LocalLogonAckMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_LOGON_ACK, 7)
        {
            Node = MessageParameters[0];
            Uid = MessageParameters.Skip(1).ToArray();

            if(Address.GetArrayValue() == 0)
            {
                NodeAddress = new byte[] { Node };
            }
            else
            {
                byte[] nodeAddress = new byte[Address.Length + 1];
                Array.Copy(Address, nodeAddress, Address.Length);
                nodeAddress[Address.Length] = Node;
                NodeAddress = nodeAddress;
            }
        }

        public byte Node { get; }

        public byte[] NodeAddress { get; }

        public byte[] Uid { get; }

        public override string ToString() => $"{base.ToString()}, Node: {Node}/{NodeExtensions.GetFullAddressString(NodeAddress)}, UID: {Uid.GetDataString()}";
    }
}