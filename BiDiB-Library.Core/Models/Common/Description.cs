using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "DecriptionType")]
public class Description
{
    [XmlAttribute("language", DataType = XmlDataTypes.Token)]
    public string Language { get; set; }

    [XmlAttribute("text", DataType = XmlDataTypes.Token)]
    public string Text { get; set; }

    [XmlText(DataType = XmlDataTypes.Token)]
    public string InnerText { get; set; }

    [XmlAttribute("help", DataType = XmlDataTypes.Token)]
    public string Help { get; set; }
}