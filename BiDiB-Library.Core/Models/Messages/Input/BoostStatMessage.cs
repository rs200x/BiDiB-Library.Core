using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_BOOST_STAT)]
public class BoostStatMessage : BiDiBInputMessage
{
    public BoostStatMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BOOST_STAT, 1)
    {
        int stateValue = MessageParameters[0];

        Control = (BoosterControl)((MessageParameters[0] & 0x70) >> 6);

        if (Control > BoosterControl.Bus)
        {
            stateValue -= 64;
        }

        State = (BoosterState)stateValue;
    }


    public BoosterState State { get; }

    public BoosterControl Control { get; }

    public override string ToString() => $"{base.ToString()}, {State}, {Control}";
}