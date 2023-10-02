namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_VENDOR_ACK)]
public class VendorAckMessage : BiDiBInputMessage
{
    public VendorAckMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_VENDOR_ACK, 1)
    {
        ConfigModeEnabled = MessageParameters[0] == 1;
    }

    public bool ConfigModeEnabled { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, ConfigMode: {ConfigModeEnabled}";
    }
}