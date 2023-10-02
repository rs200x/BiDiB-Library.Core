using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Product;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.DecoderNamespaceUrl, TypeName = "ElectricalType")]
public class Electrical
{
    [XmlAttribute("maxTotalCurrent")]
    public decimal MaxTotalCurrent { get; set; }

    [XmlAttribute("maxMotorCurrent")]
    public decimal MaxMotorCurrent { get; set; }

    [XmlAttribute("maxVoltage")]
    public decimal MaxVoltage { get; set; }
}