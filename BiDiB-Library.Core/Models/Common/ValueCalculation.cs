using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

/// <summary>
/// Class representing value calculation object
/// </summary>
[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ValueCalculationType")]
[XmlRoot("valueCalculation", Namespace = Namespaces.CommonTypesNamespaceUrl, IsNullable = false)]
public class ValueCalculation
{
    [XmlAttribute("unit")]
    public UnitType Unit { get; set; }

    [XmlIgnore]
    public int Digits { get; set; }

    [XmlAttribute("digits")]
    public string DigitsValue
    {
        get => Digits == 0 ? null : Digits.ToString();
        set => Digits = int.Parse(value);
    }

    [XmlElement("item")]
    public ValueCalculationItem[] Items { get; set; }

    [XmlElement("specialValue")]
    public SpecialValue[] SpecialValues { get; set; }
}