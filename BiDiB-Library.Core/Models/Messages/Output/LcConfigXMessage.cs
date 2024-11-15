using org.bidib.Net.Core.Models.BiDiB.Base;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class LcConfigXMessage : BiDiBOutputMessage
{
    public LcConfigXMessage(byte[] address, PortType portType, byte portNumber, params byte[] data) : base(address, BiDiBMessage.MSG_LC_CONFIGX)
    {
        PortType = portType;
        PortNumber = portNumber;
        Data = data;

        Parameters = [(byte)portType, portNumber, ..data];
    }

    public PortType PortType { get; }

    public byte PortNumber { get; }

    public byte[] Data { get; }

    public override string ToString() => $"{base.ToString()}, Port: {PortType}/{PortNumber}, Data: {Data.GetDataString()}";
}