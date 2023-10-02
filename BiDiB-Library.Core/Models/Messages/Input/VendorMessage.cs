using System;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_VENDOR)]
public class VendorMessage : BiDiBInputMessage
{
    private readonly byte[] cvNameBytes;
    private readonly byte[] cvValueBytes;

    public VendorMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_VENDOR, 2)
    {
        var nameLength = MessageParameters[0];
        cvNameBytes = new byte[nameLength];
        Array.Copy(MessageParameters, 1, cvNameBytes, 0, nameLength);

        var valueLength = MessageParameters[nameLength + 1];
        cvValueBytes = new byte[valueLength];
        Array.Copy(MessageParameters, nameLength+2, cvValueBytes, 0, valueLength);
    }

    public string CvName => cvNameBytes.GetStringValue();

    public string CvValue => cvValueBytes.GetStringValue();

    public override string ToString()
    {
        return $"{base.ToString()}, CV: {CvName}, Value: {CvValue}";
    }
}