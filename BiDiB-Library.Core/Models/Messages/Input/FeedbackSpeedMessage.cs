using System;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class FeedbackSpeedMessage : BiDiBInputMessage
    {
        public FeedbackSpeedMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_SPEED, 4)
        {
            DecoderAddress = ByteUtils.GetDecoderAddress(MessageParameters[0], MessageParameters[1]);
            Direction = MessageParameters[1].GetDecoderDirection();
            Speed = BitConverter.ToUInt16(new[] { MessageParameters[2], MessageParameters[3] }, 0);
        }

        public ushort Speed { get; }

        public ushort DecoderAddress { get; }

        public DecoderDirection Direction { get; }

        public override string ToString()
        {
            return $"{base.ToString()} Dec:{DecoderAddress} Speed:{Speed} Direction:{Direction}";
        }
    }
}