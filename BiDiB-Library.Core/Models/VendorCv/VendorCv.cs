﻿using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.BiDiB.Labels;

namespace org.bidib.netbidibc.core.Models.VendorCv
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = Namespaces.VendorCvNamespaceUrl)]
    [XmlRoot(Namespace = Namespaces.VendorCvNamespaceUrl, IsNullable = false, ElementName = "VendorCV")]
    public class VendorCv
    {
        public VersionInfo Version { get; set; }

        [XmlArrayItem("Template", IsNullable = false)]
        public Template[] Templates { get; set; }

        [XmlArray("CVDefinition")]
        [XmlArrayItem("Node", IsNullable = false)]
        public CvNode[] CvDefinition { get; set; } = Array.Empty<CvNode>();


        [XmlArrayItem("ErrorCode", IsNullable = false)]
        public ErrorCode[] ErrorCodes { get; set; }

        [XmlArray("Extension", IsNullable = true)]
        [XmlArrayItem("NodeLabels", typeof(NodeLabels))]
        public object[] Extensions { get; set; }

        //[XmlIgnore]
        public Cv[] Cvs { get; set; } = Array.Empty<Cv>();
    }


}