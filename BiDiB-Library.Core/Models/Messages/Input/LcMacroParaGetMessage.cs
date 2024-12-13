using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_LC_MACRO_PARA_GET)]
public class LcMacroParaGetMessage : BiDiBInputMessage
{
    public LcMacroParaGetMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LC_MACRO_PARA_GET, 2)
    {
        MacroNumber = MessageParameters[0];
        MacroParameter = (MacroParameter)MessageParameters[1];
    }

    public byte MacroNumber { get; }

    public MacroParameter MacroParameter { get; }

    public override string ToString() => $"{base.ToString()}, N: {MacroNumber}, P: {MacroParameter}";
}