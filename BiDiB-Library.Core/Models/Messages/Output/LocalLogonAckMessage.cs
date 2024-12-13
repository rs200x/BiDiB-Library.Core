using System.Collections.Generic;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class LocalLogonAckMessage : BiDiBOutputMessage
{
    public LocalLogonAckMessage(byte[] uniqueId) : base([0], BiDiBMessage.MSG_LOCAL_LOGON_ACK)
    {
        Node = 0;
        Uid = uniqueId;
        var parameters = new List<byte> { Node };
        parameters.AddRange(uniqueId);
        Parameters = parameters.ToArray();
    }

    public byte Node { get; }

    public byte[] Uid { get; }

    public override string ToString() => $"{base.ToString()}, Node: {Node}, UID: {Uid.GetDataString()}";
}