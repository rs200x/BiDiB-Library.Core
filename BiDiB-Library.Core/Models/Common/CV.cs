using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "CVType")]
public class Cv : CvBase
{
    private byte value;
    private byte newValue;

    [XmlElement("conditions")]
    public Conditions Conditions { get; set; }

    [XmlElement("bit", typeof(Bit))]
    [XmlElement("bitSelection", typeof(BitSelection))]
    [XmlElement("group", typeof(Group))]
    [XmlElement("partial", typeof(Partial))]
    public object[] Items { get; set; }

    [XmlAttribute("number")]
    public ushort Number { get; set; }
    
    [XmlAttribute("type")]
    public CvType Type{ get; set; }

    [XmlAttribute("indexHigh")]
    public byte IndexHigh { get; set; }

    [XmlAttribute("indexLow")]
    public byte IndexLow { get; set; }

    [XmlAttribute("initialRead")]
    public bool InitialRead { get; set; }
        
    [XmlElement("image")]
    public Image[] Images { get; set; }

    [XmlAttribute("isCustom")]
    public bool IsCustom { get; set; }

    [XmlIgnore]
    public byte Value
    {
        get => value;
        set { Set(() => Value, ref this.value, value); }
    }

    [XmlIgnore]
    public byte NewValue
    {
        get => newValue;
        set { Set(() => NewValue, ref newValue, value); }
    }

    [XmlIgnore]
    public string FullNumber => $"{Number}.{IndexHigh}.{IndexLow}";

    [XmlIgnore]
    public Description[] GroupDescriptions { get; set; }

    [XmlIgnore]
    public bool HasChanges => Value != NewValue;

    #region Overrides of Object

    public override string ToString()
    {
        return $"Number: {FullNumber}, V: {Value}, NV: {NewValue}, M: {Mode}";
    }

    #endregion
}