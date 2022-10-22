using System.Linq;

namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class SysProtocolMessage : BiDiBOutputMessage
{
    public SysProtocolMessage(byte[] address) : base(address, BiDiBMessage.MSG_SYS_P_VERSION)
    {
        Parameters = new byte[] { 0x08, 0x00 };
        Protocol = Parameters.Reverse().ToArray();
    }

    public byte[] Protocol { get; }

    public override string ToString() => $"{base.ToString()}, P: {Protocol[0]:d}.{Protocol[1]:d}";
}