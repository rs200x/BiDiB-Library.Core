using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.BiDiB.Base;

namespace org.bidib.Net.Core.Models.BiDiB.Labels;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.Labels10NamespaceUrl)]
[XmlRoot("nodeLabels", Namespace = Namespaces.Labels10NamespaceUrl, IsNullable = false)]
public class NodeLabels
{
    [XmlAttribute("defaultLabelsApplied")]
    public DefaultLabelsAction DefaultLabelsApplied { get; set; }
        
    [XmlAttribute("lang")]
    public string Language { get; set; }

    [XmlElement("nodeLabel")]
    public NodeLabel NodeLabel { get; set; }

    [XmlArray("feedbackPortLabels")]
    [XmlArrayItem("portLabel", IsNullable = false)]
    public BaseLabel[] FeedbackPortLabels { get; set; }

    [XmlArray("feedbackPositionLabels")]
    [XmlArrayItem("portLabel", IsNullable = false)]
    public BaseLabel[] FeedbackPositionLabels { get; set; }

    [XmlArray("portLabels")]
    [XmlArrayItem("portLabel", IsNullable = false)]
    public PortLabel[] PortLabels { get; set; }

    [XmlArray("macroLabels")]
    [XmlArrayItem("macroLabel", IsNullable = false)]
    public BaseLabel[] MacroLabels { get; set; }
        
    [XmlArray("accessoryLabels")]
    [XmlArrayItem("accessoryLabel", IsNullable = false)]
    public AccessoryLabel[] AccessoryLabels { get; set; }

    [XmlArray("flagLabels")]
    [XmlArrayItem("flagLabel", IsNullable = false)]
    public BaseLabel[] FlagLabels { get; set; }
}