using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class LcMacroParaMessage : BiDiBOutputMessage
{
    public LcMacroParaMessage(byte[] address, byte macroNumber, MacroParameter macroParameter, params byte[] data) : base(address, BiDiBMessage.MSG_LC_MACRO_PARA)
    {
        MacroNumber = macroNumber;
        MacroParameter = macroParameter;
        Data = data;

        Parameters = [macroNumber, (byte)macroParameter, ..data];
    }

    public byte MacroNumber { get; }

    public MacroParameter MacroParameter { get; }

    public byte[] Data { get; }

    public override string ToString() => $"{base.ToString()}, N: {MacroNumber}, P: {MacroParameter}, Data: {Data.GetDataString()}";
}