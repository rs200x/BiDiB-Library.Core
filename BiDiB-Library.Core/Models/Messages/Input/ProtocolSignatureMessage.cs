using System;
using System.Linq;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    [InputMessage(BiDiBMessage.MSG_LOCAL_PROTOCOL_SIGNATURE)]
    public class ProtocolSignatureMessage : BiDiBInputMessage
    {
        public ProtocolSignatureMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_PROTOCOL_SIGNATURE, 0)
        {
            Emitter = string.Join("", MessageParameters.Select(Convert.ToChar));
        }

        public string Emitter { get; }

        public override string ToString() => $"{base.ToString()}, EM: {Emitter}";
    }
}