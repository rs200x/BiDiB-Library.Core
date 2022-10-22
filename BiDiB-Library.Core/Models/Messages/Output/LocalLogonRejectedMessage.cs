﻿using System.Collections.Generic;
using System.Linq;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class LocalLogonRejectedMessage : BiDiBOutputMessage
    {
        public LocalLogonRejectedMessage(IEnumerable<byte> uniqueId) : base(new byte[] {0}, BiDiBMessage.MSG_LOCAL_LOGON_REJECTED)
        {
            SequenceNumber = 0;
            Parameters = uniqueId.ToArray();
            Uid = Parameters;
        }

        public byte[] Uid { get; }

        public override string ToString() => $"{base.ToString()}, UID: {Uid.GetDataString()}";
    }
}