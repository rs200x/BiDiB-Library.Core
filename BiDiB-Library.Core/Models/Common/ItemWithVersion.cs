using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common;

public class ItemWithVersion
{
    [XmlElement("version")]
    public Version Version { get; set; }
}