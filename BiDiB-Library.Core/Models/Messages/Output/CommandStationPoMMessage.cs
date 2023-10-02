using System;
using System.Globalization;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models.Common;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class CommandStationPoMMessage : BiDiBOutputMessage
{
    public CommandStationPoMMessage(byte[] address, int decoderAddress, DecoderType decoderType, string cvNumber)
        : this(address, decoderAddress, decoderType, cvNumber, CommandStationProgPoMOpCode.BIDIB_CS_POM_RD_BYTE, 0x00)
    {
    }

    public CommandStationPoMMessage(byte[] address, int decoderAddress, DecoderType decoderType, string cvNumber, byte cvValue)
        : this(address, decoderAddress, decoderType, cvNumber, CommandStationProgPoMOpCode.BIDIB_CS_POM_WR_BYTE, cvValue)
    {
    }

    public CommandStationPoMMessage(byte[] address, int decoderAddress, DecoderType decoderType, string cvNumber, CommandStationProgPoMOpCode opCode, byte cvValue)
        : base(address, BiDiBMessage.MSG_CS_POM)
    {
        CvNumber = string.IsNullOrEmpty(cvNumber) ? 1 : int.Parse(cvNumber.Split('.')[0], CultureInfo.CurrentCulture);
        FullCvNumber = cvNumber;

        DecoderAddress = decoderAddress;
        DecoderType = decoderType;
            
        CvValue = cvValue;
        Operation = opCode;
        Parameters = new byte[13];


        var addressBytes = BitConverter.GetBytes(DecoderAddress);

        Parameters[0] = addressBytes[0]; // ADDR_L
        Parameters[1] = EncodeWithDecoderType(addressBytes[1], decoderType); // ADDR_H + decoder type
        // bytes ADDR_XL, ADDR_XH, MID not in use

        Parameters[5] = (byte)opCode;

        var cvBytes = BitConverter.GetBytes(CvNumber - 1);
        Parameters[6] = cvBytes[0]; // CV_L
        Parameters[7] = cvBytes[1]; // CV_H
        // byte CV_X not in use

        // Data
        Parameters[9] = cvValue;
        // bytes 10-12 not in use
    }

    public string FullCvNumber { get; }

    public int DecoderAddress { get; }

    public DecoderType DecoderType { get; }

    public CommandStationProgPoMOpCode Operation { get; }

    public byte CvValue { get; }

    public int CvNumber { get; }

    public override string ToString()
    {
        return $"{base.ToString()} Dec:{DecoderAddress} CV:{FullCvNumber} Value:{CvValue} Type:{DecoderType} Op:{Operation}";
    }

    private static byte EncodeWithDecoderType(byte addressHighByte, DecoderType decoderType)
    {
        switch (decoderType)
        {
            case DecoderType.StandardAccessory:
            {
                return Convert.ToByte(addressHighByte | (0x01 << 6)); // define as standard accessory decoder
            }
            case DecoderType.ExtendedAccessory:
            {
                return Convert.ToByte(addressHighByte | (0x03 << 6)); // define as standard accessory decoder
            }
            default:
                // define all other as train decoder
                return addressHighByte;
        }
    }
}