﻿namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_IDENTIFY_STATE)]
public class SysIdentifyStateMessage : BiDiBInputMessage
{
    public SysIdentifyStateMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_IDENTIFY_STATE, 1)
    {
        Enabled = MessageParameters[0] == 0x01;
    }

    public bool Enabled { get; }

    public override string ToString() => $"{base.ToString()}, enabled:{Enabled}";
}