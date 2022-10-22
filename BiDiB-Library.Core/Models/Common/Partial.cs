using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    /// <summary>
    /// Class representing partial cv value object
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "PartialType")]
    public class Partial
    {
        [XmlAttribute("number")]
        public ushort Number { get; set; }

        [XmlElement("description")]
        public Description[] Descriptions { get; set; }

        [XmlAttribute("possibleValues")]
        public string PossibleValues { get; set; }

        [XmlAttribute("reset")]
        public bool Reset { get; set; }

        [XmlAttribute("disableOther")]
        public bool DisableOther { get; set; }

        [XmlAttribute("multiply")]
        public byte Multiply { get; set; }

        [XmlElement("valueCalculation")]
        public ValueCalculation ValueCalculation { get; set; }
    }
}
