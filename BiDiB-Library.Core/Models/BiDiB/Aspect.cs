using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB;

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Aspect : ModelBase
{
    private string name;

    [XmlAttribute("number")]
    public int Number { get; set; }

    
    [XmlAttribute("name")]
    public string Name
    {
        get => name;
        set => Set(() => Name, ref name, value);
    }


    [XmlAttribute("macroNumber")]
    public int MacroNumber { get; set; }

    public override string ToString()
    {
        return $"{Number:D2}: {(!string.IsNullOrEmpty(Name) ? Name : $"Aspect_{Number}")}";
    }
}