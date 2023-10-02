using System.Collections.Generic;
using System.Linq;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class LocalLogonMessage : BiDiBOutputMessage
{
    public LocalLogonMessage(IEnumerable<byte> uniqueId) : base(new byte[] {0}, BiDiBMessage.MSG_LOCAL_LOGON)
    {
        SequenceNumber = 0;
        Parameters = uniqueId.ToArray();
        Uid = Parameters;
    }

    public byte[] Uid { get; }

    public override string ToString() => $"{base.ToString()}, UID: {Uid.GetDataString()}";
}