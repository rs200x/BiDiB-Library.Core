using org.bidib.Net.Core;
using System;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_CS_DRIVE_ACK)]
public class CommandStationDriveAckMessage : BiDiBInputMessage
{
    public CommandStationDriveAckMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_CS_DRIVE_ACK, 3)
    {
        AddressLow = MessageParameters[0];
        AddressHigh = Convert.ToByte(MessageParameters[1] & 0x3f);
        Receipt = MessageParameters[2];
    }

    public byte AddressLow { get; }

    public byte AddressHigh { get; }

    public byte Receipt { get; }
}