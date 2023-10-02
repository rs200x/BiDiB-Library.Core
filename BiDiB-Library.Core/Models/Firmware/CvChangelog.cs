using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.Firmware;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "CvChangelogType")]
public class CvChangelog
{
    [XmlAttribute(DataType = XmlDataTypes.Token)]
    public string New { get; set; }

    [XmlAttribute(DataType = XmlDataTypes.Token)]
    public string Changed { get; set; }
}