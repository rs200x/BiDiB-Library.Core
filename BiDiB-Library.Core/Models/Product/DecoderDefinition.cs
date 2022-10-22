using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Common;

namespace org.bidib.netbidibc.core.Models.Product
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = Namespaces.DecoderNamespaceUrl)]
    [XmlRoot("decoderDefinition", Namespace = Namespaces.DecoderNamespaceUrl, IsNullable = false)]
    public class DecoderDefinition : ItemWithVersion
    {
        [XmlElement("decoder")]
        public Decoder Decoder { get; set; }

        public override string ToString()
        {
            return Decoder.ToString();
        }
    }
}