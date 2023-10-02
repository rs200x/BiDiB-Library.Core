using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB;

[XmlInclude(typeof(OutputSwitch))]
[XmlInclude(typeof(OutputSwitchPair))]
[XmlInclude(typeof(OutputSound))]
[XmlInclude(typeof(OutputServo))]
[XmlInclude(typeof(OutputMotor))]
[XmlInclude(typeof(OutputLight))]
[XmlInclude(typeof(OutputBacklight))]
[XmlInclude(typeof(OutputAnalog))]
[XmlInclude(typeof(InputKey))]
[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Port
{
    [XmlAttribute("number")]
    public int Number { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("disabled")]
    public bool Disabled { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class InputKey : Port
{
    [XmlAttribute("ioInputBehaviour")]
    public IoInputBehaviour IoInputBehaviour { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class OutputAnalog : Port
{
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class OutputBacklight : Port
{
    [XmlAttribute("dimmingDownSpeed")]
    public int DimmingDownSpeed { get; set; }

    [XmlAttribute("dimmingUpSpeed")]
    public int DimmingUpSpeed { get; set; }

    [XmlAttribute("channel")]
    public int Channel { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class OutputLight : Port
{

    [XmlAttribute("brightnessOff")]
    public int BrightnessOff { get; set; }


    [XmlAttribute("brightnessOn")]
    public int BrightnessOn { get; set; }


    [XmlAttribute("dimmingDownSpeed")]
    public int DimmingDownSpeed { get; set; }


    [XmlAttribute("dimmingUpSpeed")]
    public int DimmingUpSpeed { get; set; }

    [XmlAttribute("rgbValue")]
    public int Rgb { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class OutputMotor : Port
{
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class OutputServo : Port
{

    [XmlAttribute("lowerLimit")]
    public int LowerLimit { get; set; }


    [XmlAttribute("upperLimit")]
    public int UpperLimit { get; set; }


    [XmlAttribute("movingTime")]
    public int MovingTime { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class OutputSound : Port
{
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class OutputSwitch : Port
{
    [XmlAttribute("ioBehaviour")]
    public IoBehaviour IoBehaviour { get; set; }

    [XmlAttribute("switchOffTime")]
    public int SwitchOffTime { get; set; }

    [XmlAttribute("ioSwitchBehaviour")]
    public IoSwitchBehaviour IoSwitchBehaviour { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class OutputSwitchPair : Port
{
    [XmlAttribute("switchOffTime")]
    public int SwitchOffTime { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl, TypeName = "IOBehaviour")]
public enum IoBehaviour
{
    OUTPUT,
    HIGH_PULSE,
    LOW_PULSE,
    TRI_STATE,
    INPUT_PULLUP,
    INPUT_PULLDOWN,
    OUTPUT_LOW_ACTIVE,
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl, TypeName = "IOSwitchBehaviour")]
public enum IoSwitchBehaviour
{
    LOW,
    HIGH,
    Z,
    WEAK_LOW,
    WEAK_HIGH
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl, TypeName = "IOInputBehaviour")]
public enum IoInputBehaviour
{
    ACTIVE_LOW,
    ACTIVE_HIGH,
    ACTIVE_LOW_PULLUP,
    ACTIVE_HIGH_PULLDOWN
}