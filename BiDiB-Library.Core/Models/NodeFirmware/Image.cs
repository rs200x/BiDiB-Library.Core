using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.NodeFirmware;

[Serializable]
[XmlType(Namespace = Namespaces.FirmwareNamespaceUrl)]
public class Image
{
    [XmlText]
    public string FileName { get; set; }

    [XmlAttribute("format")]
    public string Format { get; set; }

    [XmlAttribute("qualifier")]
    public string Qualifier { get; set; }


}