namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_DISABLE)]

public class SysDisableMessage : BiDiBInputMessage
{
    public SysDisableMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_DISABLE, 0)
    {
        if (MessageParameters.Length < 2)
        {
            return;
        }

        ClassId = MessageParameters[0];
        ClassIdExtended = MessageParameters[1];
    }

    public byte ClassId { get; }

    public byte ClassIdExtended { get; }

    public override string ToString() => $"{base.ToString()} C:{ClassId} CE:{ClassIdExtended}";
}