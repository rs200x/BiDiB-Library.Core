namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class FeedbackConfidenceMessage : BiDiBInputMessage
    {
        public FeedbackConfidenceMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BM_CONFIDENCE, 3)
        {
            Void = MessageParameters[0];
            Freeze = MessageParameters[1];
            NoSignal = MessageParameters[2];
        }

        public byte Void { get; }
        public byte Freeze { get; }
        public byte NoSignal { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, Void: {Void}, Freeze: {Freeze}, NoSignal {NoSignal}";
        }
    }
}