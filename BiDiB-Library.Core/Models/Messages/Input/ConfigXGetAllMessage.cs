using org.bidib.Net.Core.Models.BiDiB.Base;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_LC_CONFIGX_GET_ALL)]
public class ConfigXGetAllMessage : BiDiBInputMessage
{
    public ConfigXGetAllMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LC_CONFIGX_GET_ALL, 0)
    {
        if (MessageParameters.Length == 0)
        {
            StartPort = PortType.Switch;
            StartIndex = 0;
            EndPort = PortType.All;
            EndIndex = 0xff;
        }

        if (MessageParameters.Length != 4)
        {
            return;
        }

        StartPort = (PortType)MessageParameters[0];
        StartIndex = MessageParameters[1];
        EndPort = (PortType)MessageParameters[2];
        EndIndex = MessageParameters[3];
    }

    public PortType StartPort { get; }

    public byte StartIndex { get; }

    public PortType EndPort { get; }

    public byte EndIndex { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, Start: {StartPort}/{StartIndex} End: {EndPort}/{EndIndex}";
    }
}