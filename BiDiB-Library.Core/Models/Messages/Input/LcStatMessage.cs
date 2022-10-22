using System;
using org.bidib.netbidibc.core.Models.BiDiB.Base;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class LcStatMessage : BiDiBInputMessage
    {
        public LcStatMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LC_STAT, 2)
        {
            Port = new[] { MessageParameters[0], MessageParameters[1] };
            PortType = (PortType)MessageParameters[0];
            PortNumber = MessageParameters[1];
            Status = new byte[MessageParameters.Length - 2];
            Array.Copy(MessageParameters, 2, Status, 0, Status.Length);
        }

        public byte[] Port { get; }

        public PortType PortType { get; }

        public byte PortNumber { get; }

        public byte[] Status { get; }

        public override string ToString() => $"{base.ToString()}, Port: {Port.GetDataString()}, Status: {Status.GetDataString()}";
    }
}