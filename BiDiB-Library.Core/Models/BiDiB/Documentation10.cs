using System;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB10NamespaceUrl)]
    public class Documentation10
    {
    
        [XmlAttribute("Language", DataType = XmlDataTypes.Language)]
        public string Language { get; set; }

    
        [XmlAttribute("Text")]
        public string Text { get; set; }

    
        [XmlAttribute("Description")]
        public string Description { get; set; }

    
        [XmlAttribute("Url", DataType = XmlDataTypes.AnyUri)]
        public string Url { get; set; }
    }
}