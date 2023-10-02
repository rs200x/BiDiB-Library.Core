using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB;

[XmlInclude(typeof (MacroParameterClockStart))]
[XmlInclude(typeof (MacroParameterRepeat))]
[XmlInclude(typeof (MacroParameterSlowdown))]
[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public abstract class MacroParameter
{
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class MacroParameterSlowdown : MacroParameter
{
    [XmlAttribute("speed")]
    public int Speed { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class MacroParameterRepeat : MacroParameter
{
    [XmlAttribute("repetitions")]
    public byte Repetitions { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class MacroParameterClockStart : MacroParameter
{
    [XmlAttribute("isEnabled")]
    public bool IsEnabled { get; set; }

    [XmlAttribute("weekday")]
    public string Weekday { get; set; }

    [XmlAttribute("hour")]
    public string Hour { get; set; }

    [XmlAttribute("minute")]
    public string Minute { get; set; }
}