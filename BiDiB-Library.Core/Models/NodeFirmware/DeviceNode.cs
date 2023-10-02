using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.NodeFirmware;

[Serializable]
[XmlType(Namespace = Namespaces.FirmwareNamespaceUrl)]
public class DeviceNode : Node
{
    [XmlElement("CvFilename")]
    public string CvFilename { get; set; }

    [XmlAttribute("VID")]
    public string ManufacturerId { get; set; }
        
    [XmlAttribute("EVID")]
    public string ExtendedManufacturerId { get; set; }

    [XmlAttribute("PID")]
    public string ProductId { get; set; }

    [XmlArray("NodeImages")]
    [XmlArrayItem("Image")]
    public Image[] Images { get; set; }
        
    [XmlArray("DefaultLabels")]
    [XmlArrayItem("DefaultLabelsFile")]
    public DefaultLabelFile[] DefaultLabelFiles { get; set; }
}