using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.Firmware;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "DecoderType")]
public class Decoder
{
    [XmlAttribute("name", DataType = XmlDataTypes.Token)]
    public string Name { get; set; }

    [XmlAttribute("typeIds", DataType = XmlDataTypes.Token)]
    public string TypeIds { get; set; }

    [XmlAttribute("type")]
    public DecoderType Type { get; set; }
}