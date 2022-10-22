using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB10NamespaceUrl)]
    public class Products10
    {
    
        [XmlElement("Product")]
        public Product10[] Items { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.BiDiB10NamespaceUrl)]
    public class Product10
    {
    
        [XmlElement("Documentation")]
        public Documentation10[] Documentation { get; set; }

    
        [XmlAttribute("ManufacturerId")]
        public int ManufacturerId { get; set; }

    
        [XmlAttribute("ProductTypeId")]
        public int ProductTypeId { get; set; }

    
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlIgnore]
        public string ManufacturerName { get; set; }


        public override string ToString()
        {
            return $"{ProductTypeId} - {Name} ({ManufacturerId})";
        }
    }
}