using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ProtocolType")]
    public class Protocol
    {
        [XmlAttribute("type")]
        public ProtocolType Type { get; set; }
    }
}