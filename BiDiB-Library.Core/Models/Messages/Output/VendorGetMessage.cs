using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class VendorGetMessage : BiDiBOutputMessage
{
    public VendorGetMessage(byte[] address, string cvNumber) : base(address, BiDiBMessage.MSG_VENDOR_GET)
    {
        CvNumber = cvNumber;
        Parameters = cvNumber.GetBytesWithLength();
    }

    public string CvNumber { get; }

    public override string ToString() => $"{base.ToString()}, CV: {CvNumber}";
}