using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class BoostStatMessage : BiDiBInputMessage
    {
        public BoostStatMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BOOST_STAT, 1)
        {
            State = (BoosterState)(MessageParameters[0] & 0x80);

            Control = (BoosterControl)((MessageParameters[0] & 0x70) >> 6);
        }

        public BoosterState State { get; }

        public BoosterControl Control { get; }

        public override string ToString() => $"{base.ToString()}, {State}, {Control}";
    }
}