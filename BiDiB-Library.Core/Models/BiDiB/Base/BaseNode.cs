using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB.Base
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.BiDiBBase10NamespaceUrl)]
    public class BaseNode
    {
        [XmlAttribute("userName")]
        public string UserName { get; set; }

        [XmlAttribute("productName")]
        public string ProductName { get; set; }

        [XmlAttribute("uniqueId")]
        public long UniqueId { get; set; }

        public string UniqueIdString => $"{UniqueId:X14}";
    }
}