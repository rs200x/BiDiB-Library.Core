using System.Collections.Generic;
using org.bidib.netbidibc.core.Models.Decoder;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class FeedbackAddressMessage : FeedbackMessage
    {
        public FeedbackAddressMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_ADDRESS, 1)
        {
            int addressCount = (MessageParameters.Length - 1) / 2;

            int index = 1;
            var addresses = new List<DecoderInfo>();
            for (int i = 0; i < addressCount; i++)
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
}