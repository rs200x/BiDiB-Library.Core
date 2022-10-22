using System.Linq;

namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class SysSoftwareMessage : BiDiBOutputMessage
{
    public SysSoftwareMessage(byte[] address, byte major, byte minor, byte patch) : base(address, BiDiBMessage.MSG_SYS_SW_VERSION)
    {
        Parameters = new byte[] { patch, minor, major };
        Version = Parameters.Reverse().ToArray();
    }

    public byte[] Version { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, V: {Version[0]:d}.{Version[1]:d2}.{Version[2]:d2}";
    }
}