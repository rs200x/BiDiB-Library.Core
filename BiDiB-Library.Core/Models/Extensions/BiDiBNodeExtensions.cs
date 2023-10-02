using System;
using System.Linq;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Extensions;

public static class BiDiBNodeExtensions
{
    public static string GetSoftwareVersionString(this BiDiBNode node)
    {
        return node != null ? node.SoftwareVersion.GetSoftwareVersionString() : string.Empty; 
    }

    public static string GetProtocolVersionString(this BiDiBNode node)
    {
        return node?.ProtocolVersion is not {Length: 2} 
            ? string.Empty 
            : $"{node.ProtocolVersion[0]:d}.{node.ProtocolVersion[1]:d}";
    }

    public static double GetProtocolValue(this BiDiBNode node)
    {
        if (node?.ProtocolVersion == null || !node.ProtocolVersion.Any()) { return 0; }

        return Convert.ToDouble(node.ProtocolVersion[0]) + Convert.ToDouble(node.ProtocolVersion[1]) / 10;
    }
}