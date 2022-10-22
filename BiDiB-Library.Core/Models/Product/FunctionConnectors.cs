using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Product
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderNamespaceUrl, TypeName = "FunctionConnectorsType")]
    public class FunctionConnectors
    {
        [XmlAttribute("list")]
        public string List { get; set; }
    }
}