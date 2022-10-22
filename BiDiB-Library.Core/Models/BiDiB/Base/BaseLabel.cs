using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB.Base
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.BiDiBBase10NamespaceUrl)]
    public class BaseLabel
    {
        [XmlAttribute("index")]
        public int Index { get; set; }

        [XmlAttribute("label")]
        public string Label { get; set; }
    }
}