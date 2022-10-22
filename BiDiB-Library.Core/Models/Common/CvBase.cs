using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    /// <summary>
    /// Base class for CV objects
    /// </summary>
    [Serializable]
    public class CvBase : ModelBase
    {
        [XmlElement("description")]
        public Description[] Descriptions { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("type")]
        public string TypeString { get; set; }

        [XmlAttribute("defaultValue")]
        public int DefaultValue { get; set; }

        [XmlAttribute("mode")]
        public CvMode Mode { get; set; }

        [XmlAttribute("pomWriteExclude")]
        public bool PomWriteExclude { get; set; }

        [XmlAttribute("possibleValues")]
        public string PossibleValues { get; set; }

        [XmlElement("valueCalculation")]
        public ValueCalculation ValueCalculation { get; set; }
    }
}
