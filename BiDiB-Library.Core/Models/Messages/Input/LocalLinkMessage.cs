using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_LOCAL_LINK)]
public class LocalLinkMessage : BiDiBInputMessage
{
    public LocalLinkMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_LOCAL_LINK, 2)
    {
        LinkType = (LocalLinkType)MessageParameters[0];
        Data = new byte[MessageParameters.Length - 1];
        Array.Copy(MessageParameters, 1, Data, 0, MessageParameters.Length - 1);

        switch (LinkType)
        {
            case LocalLinkType.DESCRIPTOR_PROD_STRING:
            case LocalLinkType.DESCRIPTOR_USER_STRING:
            {
                // skip type and 
                var length = MessageParameters[1];
                Data = new byte[length];
                Array.Copy(MessageParameters, 2, Data, 0, length);
                break;
            }
            case LocalLinkType.DESCRIPTOR_UID:
            case LocalLinkType.DESCRIPTOR_P_VERSION:
            case LocalLinkType.NODE_UNAVAILABLE:
            {
                Data = MessageParameters.Skip(1).ToArray();
                break;
            }
            case LocalLinkType.PAIRING_REQUEST:
            {
                Data = MessageParameters.Skip(1).Take(7).ToArray();
                RemoteId = MessageParameters.Skip(8).Take(7).ToArray();
                Timeout = MessageParameters.Last();
                break;
            }
            case LocalLinkType.STATUS_UNPAIRED:
            case LocalLinkType.STATUS_PAIRED:
            {
                Data = MessageParameters.Skip(1).Take(7).ToArray();
                RemoteId = MessageParameters.Skip(8).ToArray();
                break;
            }
            case LocalLinkType.NODE_AVAILABLE:
            {
                // no data
                break;
            }
            default:
                throw new InvalidEnumArgumentException(nameof(LinkType));
        }
    }

    public LocalLinkType LinkType { get; }

    public byte[] Data { get; }

    public byte[] RemoteId { get; }

    public byte Timeout { get; }

    public override string ToString()
    {
        string dataString;
        switch (LinkType)
        {
            case LocalLinkType.DESCRIPTOR_UID:
            {
                dataString = $"UID: {Data.GetDataString()}";
                break;
            }
            case LocalLinkType.DESCRIPTOR_PROD_STRING:
            {
                dataString = $"Prod: {Data.GetStringValue()}";
                break;
            }
            case LocalLinkType.DESCRIPTOR_USER_STRING:
            {
                dataString = $"Usr: {Data.GetStringValue()}";
                break;
            }
            case LocalLinkType.DESCRIPTOR_P_VERSION:
            {
                dataString = $"V: {string.Join(".", Data.Reverse().Select(x => x.ToString(CultureInfo.CurrentCulture)))}";
                break;
            }
            case LocalLinkType.PAIRING_REQUEST:
            {
                dataString = $"UID: {Data.GetDataString()}, L: {RemoteId.GetDataString()}, Timeout: {Timeout}";
                break;
            }
            case LocalLinkType.STATUS_UNPAIRED:
            case LocalLinkType.STATUS_PAIRED:
            {
                dataString = $"UID: {Data.GetDataString()}, L: {RemoteId.GetDataString()}";
                break;
            }

            default:
            {
                dataString = Data.GetStringValue();
                break;
            }
        }

        return $"{base.ToString()}, T: {LinkType}, {dataString}";
    }
}