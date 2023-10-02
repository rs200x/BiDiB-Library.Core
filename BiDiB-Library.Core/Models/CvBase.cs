using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models;

public class CvBase<TValue> : ModelBase
{
    private TValue newValue;
    private TValue value;

    [XmlIgnore]
    public TValue Value
    {
        get => value;
        set { Set(() => Value, ref this.value, value); }
    }

    [XmlIgnore]
    public TValue NewValue
    {
        get => newValue;
        set { Set(() => NewValue, ref newValue, value); }
    }
}