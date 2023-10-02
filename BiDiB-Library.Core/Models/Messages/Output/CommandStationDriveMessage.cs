using System;
using System.Linq;
using System.Text;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class CommandStationDriveMessage : BiDiBOutputMessage
{
    public CommandStationDriveMessage(byte[] address, byte[] parameters) : base(address, BiDiBMessage.MSG_CS_DRIVE, parameters)
    {
        DeviceAddress = BitConverter.ToInt16(Parameters.Take(2).ToArray(), 0);
    }

    public int DeviceAddress { get; }
        
    public override string ToString()
    {
        var sb = new StringBuilder(base.ToString());
        sb.Append($" Dec:{DeviceAddress}");
        for (var i = 2; i < Parameters.Length; i++)
        {
            sb.Append($" D{i - 2}:{Parameters[i]:X2}");
        }

        return sb.ToString();
    }
}