using System.Xml.Serialization;
using Newtonsoft.Json;

namespace org.bidib.Net.Core.Models.Common;

public interface ICvItem
{
    [XmlIgnore]
    [JsonProperty("type")]
    CvItemType Type { get; }
}