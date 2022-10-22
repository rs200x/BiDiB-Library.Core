using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
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
        [XmlEnum("susi")]
        Susi,
        [XmlEnum("susi-sound")]
        SusiSound,
        [XmlEnum("standardAccessory")]
        StandardAccessory,
        [XmlEnum("extendedAccessory")]
        ExtendedAccessory
    }
}