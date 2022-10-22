using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class CommandStationStateMessage : BiDiBInputMessage
    {
        public CommandStationStateMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_CS_STATE, 1)
        {
            State = (CommandStationState)MessageParameters[0];
        }

        public CommandStationState State { get;  }

        public override string ToString()
        {
            return $"{ base.ToString()}, State: {State}";
        }
    }
}