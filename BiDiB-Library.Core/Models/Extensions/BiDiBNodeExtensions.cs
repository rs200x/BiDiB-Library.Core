using System;
using System.Linq;

namespace org.bidib.netbidibc.core.Models.Extensions
{
    public static class BiDiBNodeExtensions
    {
        public static string GetSoftwareVersionString(this BiDiBNode node)
        {
            return node.SoftwareVersion is not {Length: 3} 
                ? string.Empty 
                : $"{node.SoftwareVersion[0]:d}.{node.SoftwareVersion[1]:d2}.{node.SoftwareVersion[2]:D2}";
        }

        public static string GetProtocolVersionString(this BiDiBNode node)
        {
            return node.ProtocolVersion is not {Length: 2} 
                ? string.Empty 
                : $"{node.ProtocolVersion[0]:d}.{node.ProtocolVersion[1]:d}";
        }

        public static double GetProtocolValue(this BiDiBNode node)
        {
            if (node?.ProtocolVersion == null || !node.ProtocolVersion.Any()) { return 0; }

            return Convert.ToDouble(node.ProtocolVersion[0]) + Convert.ToDouble(node.ProtocolVersion[1]) / 10;
        }
    }
}