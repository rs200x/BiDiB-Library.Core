using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "RepeaterBaseType")]
    public abstract class RepeaterBase
    {
        [XmlAttribute("count")]
        public ushort Count { get; set; }

        [XmlAttribute("itemsIndexStart")]
        public ushort ItemsIndexStart { get; set; }

        [XmlAttribute("next")]
        public byte Next { get; set; }

        [XmlAttribute("offset")]
        public ushort Offset { get; set; }
    }
}