namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class FeedbackMessage : BiDiBInputMessage
    {
        public FeedbackMessage(byte[] messageBytes): this(messageBytes, BiDiBMessage.MSG_BM_FREE, 1)
        {
        }

        public FeedbackMessage(byte[] messageBytes, BiDiBMessage expectedMessageType, int expectedParameters) 
            : base(messageBytes, expectedMessageType, expectedParameters)
        {
            FeedbackNumber = MessageParameters[0];
        }

        public byte FeedbackNumber { get; set; }

        public ushort Timestamp { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, FNum: {FeedbackNumber}, Time: {Timestamp}";
        }
    }
}