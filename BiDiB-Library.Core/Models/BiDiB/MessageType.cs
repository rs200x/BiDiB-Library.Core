using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB;

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class MessageType
{
    [XmlElement("documentation")]
    public Documentation[] Documentation { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("direction")]
    public MessageDirection Direction { get; set; }

    [XmlAttribute("category")]
    public string Category { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("answerSize")]
    public int AnswerSize { get; set; }


}

public enum MessageDirection
{
    Up,
    Down,
    Both
}