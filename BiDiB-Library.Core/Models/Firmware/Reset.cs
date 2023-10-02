using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Common;

namespace org.bidib.Net.Core.Models.Firmware;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "ResetType")]
public class Reset
{
    [XmlElement("description", Namespace = Namespaces.CommonTypesNamespaceUrl)]
    public Description[] Descriptions { get; set; }

    [XmlAttribute("cv")]
    public ushort Cv { get; set; }

    [XmlAttribute("value")]
    public byte Value { get; set; }
}