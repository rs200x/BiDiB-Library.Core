using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_CS_STATE)]
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