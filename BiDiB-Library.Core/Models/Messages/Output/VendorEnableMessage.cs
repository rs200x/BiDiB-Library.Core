using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class VendorEnableMessage : BiDiBOutputMessage
{
    public VendorEnableMessage(byte[] address, byte[] nodeUniqueId) : base(address, BiDiBMessage.MSG_VENDOR_ENABLE, nodeUniqueId)
    {
    }

    public override string ToString() => $"{base.ToString()}, Id: {Parameters.GetDataString()}";
}