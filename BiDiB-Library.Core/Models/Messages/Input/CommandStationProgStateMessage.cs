using System;
using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_CS_PROG_STATE)]
public class CommandStationProgStateMessage : BiDiBInputMessage
{
    public CommandStationProgStateMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_CS_PROG_STATE, 5)
    {
        ProgState = (CommandStationProgState)MessageParameters[0]; // Status
        Time = Convert.ToInt32(MessageParameters[1]) * 100; // value * 100ms

        CvLow = MessageParameters[2];
        CvHigh = MessageParameters[3];
        Data = MessageParameters[4]; // CV Value
    }

    public byte CvHigh { get;  }

    public byte CvLow { get; }

    public int CvNumber => CvHigh * 256 + CvLow + 1;

    public byte Data { get;  }

    public int Time { get;  }

    public CommandStationProgState ProgState { get;  }

    public override string ToString()
    {
        return $"{base.ToString()} State:{ProgState} Time:{Time} CV:{CvNumber} Data:{Data}";
    }
}