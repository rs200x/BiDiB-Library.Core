using System;
using System.Globalization;
using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models.Messages.Output
{
    public class CommandStationProgMessage : BiDiBOutputMessage
    {
        public CommandStationProgMessage(byte[] address, string cvNumber)
            : this(address, cvNumber, CommandStationProgTrackOpCode.BIDIB_CS_PROG_RD_BYTE, 0x00)
        {
        }

        public CommandStationProgMessage(byte[] address, string cvNumber, byte cvValue)
            : this(address, cvNumber, CommandStationProgTrackOpCode.BIDIB_CS_PROG_WR_BYTE, cvValue)
        {
        }

        public CommandStationProgMessage(byte[] address, string cvNumber, CommandStationProgTrackOpCode opCode, byte cvValue)
            : base(address, BiDiBMessage.MSG_CS_PROG)
        {
            CvNumber = string.IsNullOrEmpty(cvNumber) ? 1 : int.Parse(cvNumber.Split('.')[0], CultureInfo.CurrentCulture);
            FullCvNumber = cvNumber;
            CvValue = cvValue;
            Operation = opCode;

            byte[] cvBytes = BitConverter.GetBytes(CvNumber - 1); 
            
            Parameters = new byte[4];
            Parameters[0] = (byte) opCode;
            Parameters[1] = cvBytes[0]; // CV_L
            Parameters[2] = cvBytes[1]; // CV_H
            Parameters[3] = cvValue;
        }

        public CommandStationProgTrackOpCode Operation { get; }

        public byte CvValue { get; }

        public int CvNumber { get; }

        public string FullCvNumber { get; }

        public override string ToString()
        {
            return $"{base.ToString()} CV:{FullCvNumber} Value:{CvValue} Op:{Operation}";
        }
    }
}