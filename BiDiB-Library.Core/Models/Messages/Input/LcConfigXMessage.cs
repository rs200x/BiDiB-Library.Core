﻿using System;
using org.bidib.Net.Core.Models.BiDiB.Base;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_LC_CONFIGX)]
public class LcConfigXMessage : BiDiBInputMessage
{
    public LcConfigXMessage(byte[] messageBytes) : base(messageBytes,BiDiBMessage.MSG_LC_CONFIGX, 2)
    {
        Port = new[] { MessageParameters[0], MessageParameters[1] };
        PortType = (PortType)MessageParameters[0];
        PortNumber = MessageParameters[1];
        Data = new byte[MessageParameters.Length - 2];
        Array.Copy(MessageParameters, 2, Data, 0, Data.Length);
    }

    public byte[] Port { get; }

    public PortType PortType { get; }

    public byte PortNumber { get; }

    public byte[] Data { get; }

    public override string ToString() => $"{base.ToString()}, Port: {Port.GetDataString()}, Data: {Data.GetDataString()}";
}