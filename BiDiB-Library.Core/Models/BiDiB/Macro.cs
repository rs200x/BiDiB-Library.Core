using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB;

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Macros
{
    [XmlElement("macro")]
    public Macro[] Items { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Macro
{
    [XmlArray("macroParameters")]
    [XmlArrayItem("macroParameter",IsNullable = false)]
    public MacroParameter[] MacroParameters { get; set; }

    [XmlArray("macroPoints")]
    [XmlArrayItem(IsNullable = false)]
    public MacroPoint[] MacroPoints { get; set; }

    [XmlAttribute("number")]
    public int Number { get; set; }

    
    [XmlAttribute("name")]
    public string Name { get; set; }
}