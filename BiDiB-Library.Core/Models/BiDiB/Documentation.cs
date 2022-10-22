using System;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [Serializable]
    [XmlType("DocumentationType", Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class Documentation
    {
    
        [XmlAttribute("language", DataType = XmlDataTypes.Language)]
        public string Language { get; set; }

    
        [XmlAttribute("text")]
        public string Text { get; set; }

    
        [XmlAttribute("description")]
        public string Description { get; set; }

    
        [XmlAttribute("url", DataType = XmlDataTypes.AnyUri)]
        public string Url { get; set; }
    }
}