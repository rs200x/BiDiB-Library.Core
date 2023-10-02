using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB;

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Feature : ModelBase
{
    private byte featureId;
    private byte value;

    [XmlAttribute("featureCodeId")]
    public byte FeatureId
    {
        get => featureId;
        set
        {
            featureId = value;
            FeatureType = (BiDiBFeature) value;
        }
    }

    [XmlAttribute("value")]
    public byte Value
    {
        get => value;
        set { Set(() => Value, ref this.value, value); }
    }

    [XmlIgnore]
    public BiDiBFeature FeatureType { get; private set; }
}