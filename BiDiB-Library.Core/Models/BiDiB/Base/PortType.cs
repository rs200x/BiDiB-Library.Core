using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB.Base;

[Serializable]
[XmlType(Namespace = Namespaces.BiDiBBase10NamespaceUrl)]
public enum PortType :byte
{
    [XmlEnum("SWITCH")]
    Switch = 0x00,

    [XmlEnum("LIGHT")]
    Light = 0x01,

    [XmlEnum("SERVO")]
    Servo = 0x02,

    [XmlEnum("SOUND")]
    Sound = 0x03,

    [XmlEnum("MOTOR")]
    Motor = 0x04,

    [XmlEnum("ANALOGOUT")]
    AnalogOut = 0x05,

    [XmlEnum("BACKLIGHT")]
    Backlight = 0x06,

    [XmlEnum("SWITCHPAIR")]
    Switchpair = 0x07,    // width: 2, exclusive usage

    [XmlEnum("INPUT")]
    Input = 0x0f,

    [XmlIgnore]
    All = 0xff,
}