using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class AccessoryParaGetMessage : BiDiBOutputMessage
    {
        public AccessoryParaGetMessage(byte[] address, byte number, AccessoryParameter parameter) : base(address, BiDiBMessage.MSG_ACCESSORY_PARA_GET)
        {
            AccessoryNumber = number;
            Parameter = parameter;

            Parameters = new[] { number, (byte)parameter };
        }

        public byte AccessoryNumber { get; }

        public AccessoryParameter Parameter { get; }


        public override string ToString() => $"{base.ToString()} Acc:{AccessoryNumber} Param:{Parameter}";
    }
}