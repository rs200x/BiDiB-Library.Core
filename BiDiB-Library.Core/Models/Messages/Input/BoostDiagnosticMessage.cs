using System.Globalization;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_BOOST_DIAGNOSTIC)]
public class BoostDiagnosticMessage : BiDiBInputMessage
{
    public BoostDiagnosticMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_BOOST_DIAGNOSTIC, 0)
    {
        for (var k = 0; k < MessageParameters.Length; k += 2)
        {
            if (k + 1 >= MessageParameters.Length) { continue; }

            var diagnostic = (BoosterDiagnostic)MessageParameters[k];
            var value = MessageParameters[k + 1];

            switch (diagnostic)
            {
                case BoosterDiagnostic.BIDIB_BST_DIAG_I:
                    Current = value.CalculateBoosterCurrent();
                    break;
                case BoosterDiagnostic.BIDIB_BST_DIAG_V:
                    Voltage = value * 100;
                    break;
                case BoosterDiagnostic.BIDIB_BST_DIAG_TEMP:
                    Temperature = value;
                    break;
                default:
                    // diagnostic type not supported yet
                    break;
            }
        }
    }

    public int Voltage { get; set; }

    public int Temperature { get; set; }

    public int Current { get; set; }

    public override string ToString()
    {
        var currentValue = Current != 0 ? Current.ToString(CultureInfo.CurrentCulture) : "-";
        var voltageValue = Voltage != 0 ? Voltage.ToString(CultureInfo.CurrentCulture) : "-";
        var tempValue = Temperature != 0 ? Temperature.ToString(CultureInfo.CurrentCulture) : "-";
        return $"{base.ToString()}, I: {currentValue} mA, U: {voltageValue} mV, T: {tempValue} °C";
    }
}