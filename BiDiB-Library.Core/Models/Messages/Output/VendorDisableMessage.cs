using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class VendorDisableMessage : BiDiBOutputMessage
{
    public VendorDisableMessage(byte[] address, byte[] nodeUniqueId) : base(address, BiDiBMessage.MSG_VENDOR_DISABLE, nodeUniqueId)
    {
    }

    public override string ToString() => $"{base.ToString()}, Id: {Parameters.GetDataString()}";
}