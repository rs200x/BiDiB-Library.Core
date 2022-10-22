using System.Collections.Generic;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Output;

public class VendorSetMessage : BiDiBOutputMessage
{
    public VendorSetMessage(byte[] address, string cvNumber, string cvValue) : base(address, BiDiBMessage.MSG_VENDOR_SET)
    {
        CvNumber = cvNumber;
        CvValue = cvValue;
        var parameters = new List<byte>();
        parameters.AddRange(cvNumber.GetBytesWithLength());
        parameters.AddRange(cvValue.GetBytesWithLength());
        Parameters = parameters.ToArray();
    }

    public string CvNumber { get; }

    public string CvValue { get; }

    public override string ToString() => $"{base.ToString()}, CV: {CvNumber}, Value: {CvValue}";
}