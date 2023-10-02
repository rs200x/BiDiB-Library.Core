using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.BiDiB.Base;

namespace org.bidib.Net.Core.Models.BiDiB.Labels;

[Serializable]
[DebuggerStepThrough]
[XmlType(AnonymousType = true, Namespace = Namespaces.Labels10NamespaceUrl, TypeName = "nodeLabel")]
[XmlRoot(Namespace = Namespaces.Labels10NamespaceUrl, IsNullable = false)]
public class NodeLabel : BaseNode
{
}