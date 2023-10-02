using System;
using System.Linq;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_LOCAL_LOGON_ACK)]
public class LocalLogonAckMessage : BiDiBInputMessage
{
    public LocalLogonAckMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_LOGON_ACK, 7)
    {
        Node = MessageParameters[0];
        Uid = MessageParameters.Skip(1).ToArray();

        if(Address.GetArrayValue() == 0)
        {
            NodeAddress = new[] { Node };
        }
        else
        {
            var nodeAddress = new byte[Address.Length + 1];
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