using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    [XmlRoot(Namespace = Namespaces.BiDiB20NamespaceUrl, IsNullable = true, ElementName = "BiDiB")]
    public class BiDiBRoot
    {
        [XmlElement("accessories", typeof(Accessories))]
        [XmlElement("macros", typeof(Macros))]
        [XmlElement("nodes", typeof(Nodes))]
        [XmlElement("products", typeof(Products))]
        [XmlElement("protocol", typeof(Protocol))]
        public object Item { get; set; }

        [XmlAttribute("schemaVersion")]
        public decimal SchemaVersion { get; set; }
    }
}