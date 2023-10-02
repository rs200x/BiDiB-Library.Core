using System.Linq;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class SysProtocolMessage : BiDiBOutputMessage
{
    public SysProtocolMessage(byte[] address, byte major, byte minor) : base(address, BiDiBMessage.MSG_SYS_P_VERSION)
    {
        Parameters = new[] { minor, major };
        Protocol = Parameters.Reverse().ToArray();
    }

    public byte[] Protocol { get; }

    public override string ToString() => $"{base.ToString()}, P: {Protocol[0]:d}.{Protocol[1]:d}";
}