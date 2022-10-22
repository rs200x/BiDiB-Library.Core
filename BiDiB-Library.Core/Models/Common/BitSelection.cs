using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "BitSelectionType")]
    public class BitSelection
    {
        [XmlElement("option")]
        public Option[] Options { get; set; }

        [XmlAttribute("number")]
        public byte Number { get; set; }
    }
}