using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Common;
using Cv = org.bidib.Net.Core.Models.Common.Cv;

namespace org.bidib.Net.Core.Models.Firmware;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "CategoryType")]
public class Category
{
    [XmlElement("description", Namespace = Namespaces.CommonTypesNamespaceUrl)]
    public Description[] Descriptions { get; set; }

    [XmlElement("idReference", typeof(CvReference))]
    [XmlElement("category", typeof(Category))]
    public object[] Items { get; set; }

    public IEnumerable<Cv> AllCvs
    {
        get
        {
            if (Items == null)
            {
                return new List<Cv>();
            }

            var allCvs = Items.OfType<CvReference>().SelectMany(x => x.AllCvs).ToList();
            allCvs.AddRange(Items.OfType<Category>().SelectMany(x => x.AllCvs));
            return allCvs.OrderBy(x => x.Number);
        }
    }
}