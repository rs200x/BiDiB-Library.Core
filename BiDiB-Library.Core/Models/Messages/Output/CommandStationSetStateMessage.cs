using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class CommandStationSetStateMessage : BiDiBOutputMessage
{
    public CommandStationSetStateMessage( CommandStationState state, byte[] address) : base(address, BiDiBMessage.MSG_CS_SET_STATE)
    {
        State = state;
        Parameters = new[] { (byte)state };
    }

    public CommandStationState State { get; }

    public override string ToString()
    {
        return $"{base.ToString()} State:{State}";
    }
}