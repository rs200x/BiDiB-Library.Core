using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class ProtocolSignatureMessage : BiDiBOutputMessage
    {
        public ProtocolSignatureMessage(string emitter) : base(new byte[] { 0 }, BiDiBMessage.MSG_LOCAL_PROTOCOL_SIGNATURE)
        {
            Parameters = emitter.GetBytes();
            Emitter = emitter;
        }

        public string Emitter { get; }

        public override string ToString() => $"{base.ToString()}, EM: {Emitter}";
    }
}