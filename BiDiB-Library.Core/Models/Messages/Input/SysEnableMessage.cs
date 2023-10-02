namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_ENABLE)]
public class SysEnableMessage : BiDiBInputMessage
{
    public SysEnableMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_ENABLE, 0)
    {
        if (MessageParameters.Length < 2)
        {
            return;
        }

        ClassId = MessageParameters[0];
        ClassIdExtended = MessageParameters[1];
    }

    public byte ClassId { get; }

    public byte ClassIdExtended { get;  }

    public override string ToString() => $"{base.ToString()} C:{ClassId} CE:{ClassIdExtended}";
}