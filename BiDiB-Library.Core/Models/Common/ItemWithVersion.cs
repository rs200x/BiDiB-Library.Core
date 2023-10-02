using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

public class ItemWithVersion
{
    [XmlElement("version")]
    public Version Version { get; set; }
}