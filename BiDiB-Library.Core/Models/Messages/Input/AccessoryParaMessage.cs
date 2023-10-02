using System;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_ACCESSORY_PARA)]
public class AccessoryParaMessage : BiDiBInputMessage
{
    public AccessoryParaMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_ACCESSORY_PARA, 2)
    {
        Number = MessageParameters[0];

        if (MessageParameters[1] == (byte)AccessoryParameter.ACCESSORY_PARA_NOTEXIST)
        {
            Parameter = (AccessoryParameter)MessageParameters[1];
        }
        else
        {
            Parameter = (AccessoryParameter)MessageParameters[1];
            if (MessageParameters.Length <= 2){return;}

            Data = new byte[MessageParameters.Length - 2];
            Array.Copy(MessageParameters, 2, Data, 0, Data.LongLength);
        }
    }

    public byte Number { get; }

    public AccessoryParameter Parameter { get; }

    public byte[] Data { get; }

    public override string ToString() => $"{base.ToString()}, Accessory: {Number}, Parameter: {Parameter}, Data: {Data.GetDataString()}";
}