namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class AccessorySetMessage : BiDiBOutputMessage
    {
        public AccessorySetMessage(byte[] address, byte accessoryNumber, byte aspectNumber) : base(address, BiDiBMessage.MSG_ACCESSORY_SET)
        {
            Parameters = new[] { accessoryNumber, aspectNumber };
        }
    }
}