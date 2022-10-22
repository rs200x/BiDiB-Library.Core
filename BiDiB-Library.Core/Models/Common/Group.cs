using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "GroupType")]
    public class Group
    {
        [XmlElement("description")]
        public Description[] Description { get; set; }

        [XmlElement("option")]
        public Option[] Options { get; set; }

        [XmlAttribute("number")]
        public byte Number { get; set; }
    }
}