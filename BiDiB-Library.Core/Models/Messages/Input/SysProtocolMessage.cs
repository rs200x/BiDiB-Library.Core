namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_P_VERSION)]
public class SysProtocolMessage : BiDiBInputMessage
{
    public SysProtocolMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_P_VERSION, 2)
    {
        Protocol = new[] { MessageParameters[1], MessageParameters[0] };
    }

    public byte[] Protocol { get; }

    public override string ToString() => $"{base.ToString()}, P: {Protocol[0]:d}.{Protocol[1]:d}";
}