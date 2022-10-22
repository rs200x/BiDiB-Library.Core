using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class Products
    {
    
        [XmlElement("product")]
        public Product[] Items { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class Product
    {
    
        [XmlElement("documentation")]
        public Documentation[] Documentation { get; set; }

    
        [XmlAttribute("manufacturerId")]
        public int ManufacturerId { get; set; }

    
        [XmlAttribute("productTypeId")]
        public int ProductTypeId { get; set; }

    
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlIgnore]
        public string ManufacturerName { get; set; }
    }
}