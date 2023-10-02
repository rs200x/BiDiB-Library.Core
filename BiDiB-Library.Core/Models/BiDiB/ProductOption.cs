using System.Runtime.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB;

[DataContract]
public class ProductOption
{
    [DataMember(Name = "vid")]
    public int ManufacturerId { get; set; }

    [DataMember(Name = "evid")]
    public int ExtendedManufacturerId { get; set; }

    [DataMember(Name = "pid")]
    public int ProductId { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "comment")]
    public string Comment { get; set; }
}