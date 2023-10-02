using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using org.bidib.Net.Core.Models.Common;

namespace org.bidib.Net.Core.Models.VendorCv.Extensions;

public static class VendorCvExtensions
{
    public static CvNode FindNode(this VendorCv vendorCv, string descriptionPath)
    {
        var path = !string.IsNullOrEmpty(descriptionPath) ? descriptionPath.Split('/') : Array.Empty<string>();

        return vendorCv?.CvDefinition?.Select(cvNode => FindChildNode(cvNode, path)).FirstOrDefault(foundNode => foundNode != null);
    }

    public static Cv FindCv(this VendorCv vendorCv, string keyword)
    {
        return vendorCv?.Cvs != null ? Array.Find( vendorCv.Cvs, x => x.Keyword == keyword) : null;
    }

    public static IEnumerable<Cv> FindCvs(this VendorCv vendorCv, string keyword)
    {
        return vendorCv?.Cvs?.Where(x => x.Keyword == keyword) ?? Enumerable.Empty<Cv>();
    }

    public static IEnumerable<Cv> GetCvsWithChanges(this VendorCv vendorCv)
    {
        return vendorCv?.Cvs?.Where(x => !string.IsNullOrEmpty(x.NewValue) && x.Value != x.NewValue).ToList() ?? Enumerable.Empty<Cv>();
    }

    public static uint GetUIntValue(this VendorCv vendorCv, string keyword)
    {
        if (vendorCv?.Cvs == null) { return default; }

        IEnumerable<Cv> cvs = vendorCv.Cvs.Where(x => x.Keyword == keyword).ToList();
        if (!cvs.Any()) { return 0;}
        var valueBytes = cvs.Select(cv => Convert.ToByte(cv.Value, CultureInfo.CurrentCulture)).ToArray();
        return BitConverter.ToUInt32(valueBytes, 0);
    }

    public static byte GetByteValue(this VendorCv vendorCv, string keyword)
    {
        if (vendorCv?.Cvs == null) { return default; }
        var cv = Array.Find(vendorCv.Cvs, x => x.Keyword == keyword);
        return cv != null ? Convert.ToByte(cv.Value, CultureInfo.CurrentCulture) : (byte)0;
    }

    public static ushort GetUShortValue(this VendorCv vendorCv, string keywordLow, string keywordHigh)
    {
        if (vendorCv?.Cvs == null || string.IsNullOrEmpty(keywordLow) || string.IsNullOrEmpty(keywordHigh)) { return default; }

        var cvLow = Array.Find(vendorCv.Cvs, x => x.Keyword == keywordLow);
        var cvHigh = Array.Find(vendorCv.Cvs, x => x.Keyword == keywordHigh);
        return BitConverter.ToUInt16(new[] { Convert.ToByte(cvLow?.Value, CultureInfo.CurrentCulture), Convert.ToByte(cvHigh?.Value, CultureInfo.CurrentCulture) }, 0);
    }

    public static ushort GetUShortValue(this VendorCv vendorCv, string keywordLow)
    {
        if(vendorCv?.Cvs == null || string.IsNullOrEmpty(keywordLow)) { return default; }

        var cvLow = Array.Find(vendorCv.Cvs, x => x.Keyword == keywordLow);
        var cvHigh = vendorCv.Cvs[vendorCv.Cvs.ToList().IndexOf(cvLow) + 1];
        return BitConverter.ToUInt16(new[] { Convert.ToByte(cvLow?.Value, CultureInfo.CurrentCulture), Convert.ToByte(cvHigh?.Value, CultureInfo.CurrentCulture) }, 0);
    }

    public static void SetValue(this VendorCv vendorCv, byte value, string keyword)
    {
        if (vendorCv?.Cvs == null || string.IsNullOrEmpty(keyword)) { return; }

        var cv = Array.Find(vendorCv.Cvs, x => x.Keyword == keyword);
        if (cv != null)
        {
            cv.NewValue = value.ToString(CultureInfo.CurrentCulture);
        }
    }

    public static void SetValue(this VendorCv vendorCv, ushort value, string keywordLow, string keywordHigh)
    {
        if (vendorCv?.Cvs == null || string.IsNullOrEmpty(keywordLow) || string.IsNullOrEmpty(keywordHigh)) { return; }
        var cvLow = Array.Find(vendorCv.Cvs, x => x.Keyword == keywordLow);
        var cvHigh = Array.Find(vendorCv.Cvs, x => x.Keyword == keywordHigh);
        SetValues(cvLow, cvHigh, value);
    }

    public static void SetValue(this VendorCv vendorCv, ushort value, string keywordLow)
    {
        if (vendorCv?.Cvs == null || string.IsNullOrEmpty(keywordLow)) { return; }
        var cvLow = Array.Find(vendorCv.Cvs, x => x.Keyword == keywordLow);
        var cvHigh = vendorCv.Cvs[vendorCv.Cvs.ToList().IndexOf(cvLow) + 1];
        SetValues(cvLow, cvHigh, value);
    }

    public static void SetValue(this VendorCv vendorCv, uint value, string keyword)
    {
        if (vendorCv?.Cvs == null || string.IsNullOrEmpty(keyword)) { return; }
        var cvs = vendorCv.Cvs.Where(x => x.Keyword == keyword).ToList();
        var valueBytes = BitConverter.GetBytes(value);
        if (cvs.Count != valueBytes.Length || cvs.Count != 4)
        {
            return;
        }

        for (var i = 0; i < 4; i++)
        {
            cvs[i].NewValue = valueBytes[i].ToString(CultureInfo.CurrentCulture);
        }
    }

    public static CvMode ToCvMode(this CvAccessMode accessMode)
    {
        switch (accessMode)
        {
            case CvAccessMode.ReadWrite:
                return CvMode.ReadWrite;
            case CvAccessMode.ReadOnly:
                return CvMode.ReadOnly;
            case CvAccessMode.WriteOnly:
                return CvMode.WriteOnly;
            case CvAccessMode.Write:
                return CvMode.WriteOnly;
            case CvAccessMode.Hidden:
                return CvMode.ReadOnly;
            default:
                throw new ArgumentOutOfRangeException(nameof(accessMode), accessMode, null);
        }
    }

    private static CvNode FindChildNode(CvNode node, string[] path)
    {
        if (path.Length == 0) { return null; }

        var currentLevel = path.FirstOrDefault();

        if (string.IsNullOrEmpty(currentLevel)) { return null; }

        CvNode foundNode = null;

        var match = DescriptionMatches(node, currentLevel);

        if (match)
        {
            foundNode = node;
        }


        if (!match && node.Nodes != null)
        {
            foreach (var childNode in node.Nodes)
            {
                if (!DescriptionMatches(childNode, currentLevel))
                {
                    continue;
                }

                foundNode = childNode;
                break;
            }
        }

        if (foundNode == null || path.Length <= 1) { return foundNode; }

        var subPath = new string[path.Length - 1];
        Array.Copy(path, 1, subPath, 0, subPath.Length);
        return FindChildNode(foundNode, subPath);
    }

    private static bool DescriptionMatches(CvNode node, string description)
    {
        return node.Description != null && Array.Exists(node.Description,cvDescription => cvDescription.Text == description);
    }

    private static void SetValues(CvBase<string> cvLow, CvBase<string> cvHigh, ushort value)
    {
        var valueBytes = BitConverter.GetBytes(value);
        if (cvLow != null)
        {
            cvLow.NewValue = valueBytes[0].ToString(CultureInfo.CurrentCulture);
        }

        if (cvHigh != null)
        {
            cvHigh.NewValue = valueBytes[1].ToString(CultureInfo.CurrentCulture);
        }
    }
}