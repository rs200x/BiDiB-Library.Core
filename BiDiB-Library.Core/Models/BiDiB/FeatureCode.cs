using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class FeatureCode
    {
        private int id;

        [XmlElement("documentation")]
        public Documentation[] Documentation { get; set; }

        [XmlAttribute("id")]
        public int Id
        {
            get => id;
            set
            {
                id = value;
                FeatureType = (BiDiBFeature) value;
            }
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("unit")]
        public string Unit { get; set; }

        [XmlIgnore]
        public BiDiBFeature FeatureType { get; private set; }
    }
}