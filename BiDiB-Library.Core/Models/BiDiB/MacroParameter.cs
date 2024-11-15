using System;
using System.Xml.Serialization;
using MacroParameterType = org.bidib.Net.Core.Enumerations.MacroParameter;

namespace org.bidib.Net.Core.Models.BiDiB;

[XmlInclude(typeof (MacroParameterClockStart))]
[XmlInclude(typeof (MacroParameterRepeat))]
[XmlInclude(typeof (MacroParameterSlowdown))]
//[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public interface IMacroParameter
{
    public MacroParameterType Type { get; }
    public byte[] Value { get; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class MacroParameterSlowdown : IMacroParameter
{
    public MacroParameterType Type => MacroParameterType.MACRO_PARA_SLOWDOWN;

    [XmlAttribute("speed")]
    public byte Speed { get; set; }

    public byte[] Value => [Speed];
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class MacroParameterRepeat : IMacroParameter
{
    public MacroParameterType Type => MacroParameterType.MACRO_PARA_REPEAT;

    [XmlAttribute("repetitions")]
    public byte Repetitions { get; set; }

    public byte[] Value => [Repetitions];
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class MacroParameterClockStart : IMacroParameter
{
    public MacroParameterType Type => MacroParameterType.MACRO_PARA_START_CLK;

    [XmlAttribute("isEnabled")]
    public bool IsEnabled { get; set; }

    [XmlAttribute("weekday")]
    public string Weekday { get; set; }

    [XmlAttribute("hour")]
    public string Hour { get; set; }

    [XmlAttribute("minute")]
    public string Minute { get; set; }

    public byte[] Value => [0x3f, 0xbf, 0x7f, 0xff];
}