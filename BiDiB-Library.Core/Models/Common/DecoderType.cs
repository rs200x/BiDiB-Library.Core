using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "DecoderTypeType")]
public enum DecoderType
{
    [XmlEnum("loco")]
    Loco,
    [XmlEnum("loco-sound")]
    LocoSound,
    [XmlEnum("function")]
    Function,
    [XmlEnum("car")]
    Car,
    [XmlEnum("car-sound")]
    CarSound,
    [XmlEnum("susi")]
    Susi,
    [XmlEnum("susi-sound")]
    SusiSound,
    [XmlEnum("standardAccessory")]
    StandardAccessory,
    [XmlEnum("extendedAccessory")]
    ExtendedAccessory,

}