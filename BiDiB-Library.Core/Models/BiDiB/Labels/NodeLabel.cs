using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.BiDiB.Base;

namespace org.bidib.netbidibc.core.Models.BiDiB.Labels
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = Namespaces.Labels10NamespaceUrl, TypeName = "nodeLabel")]
    [XmlRoot(Namespace = Namespaces.Labels10NamespaceUrl, IsNullable = false)]
    public class NodeLabel : BaseNode
    {
    }
}