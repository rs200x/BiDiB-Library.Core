using System.Collections.Generic;
using org.bidib.Net.Core.Models.Decoder;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_BM_ADDRESS)]
public class FeedbackAddressMessage : FeedbackMessage
{
    public FeedbackAddressMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_ADDRESS, 1)
    {
        var addressCount = (MessageParameters.Length - 1) / 2;

        var index = 1;
        var addresses = new List<DecoderInfo>();
        for (var i = 0; i < addressCount; i++)
        {
            var address = ByteUtils.GetDecoderAddress(MessageParameters[index], MessageParameters[index + 1]);
            var direction = MessageParameters[index + 1].GetDecoderDirection();
            addresses.Add(new DecoderInfo(address, direction));
            index += 2;
        }

        Addresses = addresses;
    }

    public IEnumerable<DecoderInfo> Addresses { get; }

    public override string ToString() => $"{base.ToString()}, Addresses: {string.Join(";", Addresses)}";
}