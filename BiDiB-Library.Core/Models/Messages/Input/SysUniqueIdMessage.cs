using System;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_UNIQUE_ID)]
public class SysUniqueIdMessage : BiDiBInputMessage
{
    public SysUniqueIdMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_UNIQUE_ID, 7)
    {
        UniqueId = new byte[7];
        Array.Copy(MessageParameters, 0, UniqueId, 0, 7);

        ClassId = new[] { MessageParameters[0], MessageParameters[1] };
        VendorId = MessageParameters[2];
        ProducerId = new[] { MessageParameters[3], MessageParameters[4], MessageParameters[5], MessageParameters[6] };

        Fingerprint = 0;
        if (MessageParameters.Length == 11)
        {
            Fingerprint = BitConverter.ToInt32(MessageParameters, 7);
        }
    }

    public int Fingerprint { get; set; }

    public byte[] UniqueId { get; }

    public byte[] ProducerId { get; }

    public byte VendorId { get;  }

    public byte[] ClassId { get;  }

    public string VendorProducerId => $"V {VendorId:X2} P {ProducerId[0]:X2}{ProducerId[1]:X2}{ProducerId[2]:X2}{ProducerId[3]:X2}";

    public string HexUId => $"{ClassId[0]:X2}.{ClassId[1]:X2}.{VendorId:X2}.{ProducerId[0]:X2}.{ProducerId[1]:X2}.{ProducerId[2]:X2}.{ProducerId[3]:X2}";

    public override string ToString()
    {
        return $"{base.ToString()}, ID:{HexUId}, FP:{Fingerprint}";
    }
}