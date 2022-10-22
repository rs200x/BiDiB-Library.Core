using System;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class FeedbackOccupiedMessage : FeedbackMessage
    {
        public FeedbackOccupiedMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_OCC, 1)
        {
            if (MessageParameters.Length == 3)
            {
                Timestamp = BitConverter.ToUInt16(MessageParameters, 1);
            }
        }
    }
}