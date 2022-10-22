using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models.Messages.Output;

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