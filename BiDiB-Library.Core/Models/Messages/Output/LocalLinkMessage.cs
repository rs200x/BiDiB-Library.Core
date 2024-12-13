using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class LocalLinkMessage : BiDiBOutputMessage
{
    public LocalLinkMessage(LocalLinkType linkType, ICollection<byte> data) : base([0], BiDiBMessage.MSG_LOCAL_LINK)
    {
        SequenceNumber = 0;
        LinkType = linkType;
        var parameters = new List<byte> { (byte)LinkType };

        if (LinkType == LocalLinkType.DESCRIPTOR_PROD_STRING || linkType == LocalLinkType.DESCRIPTOR_USER_STRING)
        {
            parameters.Add(Convert.ToByte(data.Count));
        }

        if (LinkType == LocalLinkType.DESCRIPTOR_P_VERSION)
        {
            data = data.Reverse().ToList();
        }

        if (LinkType == LocalLinkType.PAIRING_REQUEST)
        {
            Timeout = data.Last();
        }

        parameters.AddRange(data);
        Parameters = parameters.ToArray();
    }

    public LocalLinkType LinkType { get; }

    public byte Timeout { get; }

    public override string ToString()
    {
        string dataString;
        var data = Parameters.Skip(1).ToArray();
        switch (LinkType)
        {
            case LocalLinkType.DESCRIPTOR_UID:
            {
                dataString = $"UID: {data.GetDataString()}";
                break;
            }
            case LocalLinkType.DESCRIPTOR_PROD_STRING:
            {
                dataString = $"Prod: {data.Skip(1).ToArray().GetStringValue()}";
                break;
            }
            case LocalLinkType.DESCRIPTOR_USER_STRING:
            {
                dataString = $"Usr: {data.Skip(1).ToArray().GetStringValue()}";
                break;
            }
            case LocalLinkType.DESCRIPTOR_P_VERSION:
            {
                dataString = $"V: {string.Join(".", data.Reverse().Select(x => x.ToString(CultureInfo.CurrentCulture)))}";
                break;
            }
            case LocalLinkType.PAIRING_REQUEST:
            {
                dataString = $"LID: {data.Take(7).ToArray().GetDataString()}, RID: {data.Skip(7).Take(7).ToArray().GetDataString()}, Timeout: {Timeout}";
                break;
            }
            case LocalLinkType.STATUS_UNPAIRED:
            case LocalLinkType.STATUS_PAIRED:
            {
                dataString = $"LID: {data.Take(7).ToArray().GetDataString()}, RID: {data.Skip(7).ToArray().GetDataString()}";
                break;
            }
            default:
            {
                dataString = "Don't send this!";
                break;
            }

        }

        return $"{base.ToString()}, T: {LinkType}, {dataString}";
    }
}