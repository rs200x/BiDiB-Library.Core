using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [XmlInclude(typeof(MacroPointOutput))]
    [XmlInclude(typeof(MacroPointOutputSwitch))]
    [XmlInclude(typeof(MacroPointOutputSound))]
    [XmlInclude(typeof(MacroPointOutputServo))]
    [XmlInclude(typeof(MacroPointOutputMotor))]
    [XmlInclude(typeof(MacroPointOutputLight))]
    [XmlInclude(typeof(MacroPointOutputBacklight))]
    [XmlInclude(typeof(MacroPointOutputAnalog))]
    [XmlInclude(typeof(MacroPointMacro))]
    [XmlInclude(typeof(MacroPointInput))]
    [XmlInclude(typeof(MacroPointFlag))]
    [XmlInclude(typeof(MacroPointDelay))]
    [XmlInclude(typeof(MacroPointCriticalSection))]
    [XmlInclude(typeof(MacroPointAccessoryNotification))]
    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public abstract class MacroPoint
    {
        [XmlAttribute("number")]
        public int Number { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointAccessoryNotification : MacroPoint
    {
        [XmlAttribute("function")]
        public FunctionAccessoryNotification Function { get; set; }

        [XmlAttribute("inputNumber")]
        public int InputNumber { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public enum FunctionAccessoryNotification
    {
        Okay,
        OkayIfInput0,
        OkayIfInput1
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointCriticalSection : MacroPoint
    {
        [XmlAttribute("function")]
        public FunctionCriticalSection Function { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public enum FunctionCriticalSection
    {
        Begin,
        End
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointServoMoveQuery : MacroPoint
    {
        [XmlAttribute("outputNumber")]
        public int OutputNumber { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointDelay : MacroPoint
    {
        [XmlAttribute("delay")]
        public int Delay { get; set; }

        [XmlAttribute("function")]
        public FunctionDelay Function { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public enum FunctionDelay
    {
        Fixed,
        Random
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointFlag : MacroPoint
    {
        [XmlAttribute("flagNumber")]
        public int FlagNumber { get; set; }

        [XmlAttribute("function")]
        public FunctionFlag Function { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public enum FunctionFlag
    {
        Set,
        Reset,
        Query0,
        Query1
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointInput : MacroPoint
    {
        [XmlAttribute("inputNumber")]
        public int InputNumber { get; set; }

        [XmlAttribute("function")]
        public FunctionInput Function { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public enum FunctionInput
    {
        WaitFor0,
        WaitFor1
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointMacro : MacroPoint
    {
        [XmlAttribute("macroNumber")]
        public int MacroNumber { get; set; }

        [XmlAttribute("function")]
        public FunctionMacro Function { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public enum FunctionMacro
    {
        Start,
        Stop
    }

    [XmlInclude(typeof(MacroPointOutputSwitch))]
    [XmlInclude(typeof(MacroPointOutputSound))]
    [XmlInclude(typeof(MacroPointOutputServo))]
    [XmlInclude(typeof(MacroPointOutputMotor))]
    [XmlInclude(typeof(MacroPointOutputLight))]
    [XmlInclude(typeof(MacroPointOutputBacklight))]
    [XmlInclude(typeof(MacroPointOutputAnalog))]
    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public abstract class MacroPointOutput : MacroPoint
    {
        [XmlAttribute("delay")]
        public int Delay { get; set; }

        [XmlAttribute("outputNumber")]
        public int OutputNumber { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointOutputAnalog : MacroPointOutput
    {
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointOutputBacklight : MacroPointOutput
    {
        [XmlAttribute("brightness")]
        public int Brightness { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointOutputLight : MacroPointOutput
    {
        [XmlAttribute("function")]
        public FunctionOutputLight Function { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointOutputSwitchPair : MacroPointOutput
    {
        [XmlAttribute("function")]
        public FunctionOutputSwitch Function { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public enum FunctionOutputLight
    {
        TurnOff,
        TurnOn,
        DimDown,
        DimUp,
        NeonFlicker,
        BlinkA,
        BlinkB,
        FlashA,
        FlashB,
        DoubleFlash
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointOutputMotor : MacroPointOutput
    {
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointOutputServo : MacroPointOutput
    {
        [XmlAttribute("position")]
        public int Position { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointOutputSound : MacroPointOutput
    {
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class MacroPointOutputSwitch : MacroPointOutput
    {
        [XmlAttribute("function")]
        public FunctionOutputSwitch Function { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public enum FunctionOutputSwitch
    {
        On,
        Off
    }
}