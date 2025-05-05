using System.Xml.Serialization;
using Newtonsoft.Json;

namespace org.bidib.Net.Core.Models.Common;

public abstract class CvItem
{
    [XmlIgnore]
    [JsonProperty("type")]
    public abstract CvItemType Type { get; }
}