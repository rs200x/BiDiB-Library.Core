using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB10NamespaceUrl)]
    [XmlRoot(Namespace = Namespaces.BiDiB10NamespaceUrl, IsNullable = true, ElementName = "BiDiB")]
    public class BiDiB10Root
    {
        [XmlElement("Accessories", typeof(Accessories))]
        [XmlElement("Macros", typeof(Macros))]
        [XmlElement("Nodes", typeof(Nodes))]
        [XmlElement("Products", typeof(Products10))]
        [XmlElement("Protocol", typeof(Protocol))]
        public object Item { get; set; }

        [XmlAttribute("SchemaVersion")]
        public decimal SchemaVersion { get; set; }
    }
}