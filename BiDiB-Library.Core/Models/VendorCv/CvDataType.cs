using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{

    [Serializable]
    [XmlType(Namespace = Namespaces.VendorCvNamespaceUrl)]
    public enum CvDataType
    {
    
        Byte,

    
        Int,

    
        Bit,

    
        DCC_ADDR_RG,

    
        GBM16TReverser,

    
        SignedChar,

    
        Radio,

    
        Long,

        DccLongAddr,

        DccAccAddr,

        String
    }
}