using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl)]
public enum UnitType
{
    [XmlEnum("-")]
    NoUnit,
    [XmlEnum("sec")]
    Second,
    [XmlEnum("min")]
    Minute,
    [XmlEnum("ms")]
    Millisecond,
    [XmlEnum("us")]
    Microsecond,
    [XmlEnum("%")]
    Percent,
    [XmlEnum("V")]
    Volts,
    [XmlEnum("m")]
    Meter,
    [XmlEnum("°C")]
    DegreeCelcius,
    [XmlEnum("km/h")]
    KilometerPerHour,
    [XmlEnum("cm")]
    Centimeter,
    [XmlEnum("mm")]
    Millimeter,
    [XmlEnum("kw")]
    Kilowatts,
    [XmlEnum("t")]
    Tonnes,
    [XmlEnum("mm/s")]
    MillimeterPerSecond,
    [XmlEnum("rad/s")]
    RadPerSecond,
    [XmlEnum("mA")]
    MilliAmpere,
    [XmlEnum("Ohm")]
    Ohm,
    [XmlEnum("mOhm")]
    MilliOhm,
    [XmlEnum("Hz")]
    Hertz,
    [XmlEnum("kHz")]
    KiloHertz,
    [XmlEnum("rpm")]
    RevolutionsPerMinute,
    [XmlEnum("rpm/V")]
    RevolutionsPerMinutePerVolt,
    [XmlEnum("h")]
    Henry
}