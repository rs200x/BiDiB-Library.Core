using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using org.bidib.netbidibc.core.Models.Common;

namespace org.bidib.netbidibc.core.Models.VendorCv.Extensions
{
    public static class VendorCvExtensions
    {
        public static CvNode FindNode(this VendorCv vendorCv, string descriptionPath)
        {
            string[] path = !string.IsNullOrEmpty(descriptionPath) ? descriptionPath.Split('/') : new string[0];

            return vendorCv.CvDefinition.Select(cvNode => FindChildNode(cvNode, path)).FirstOrDefault(foundNode => foundNode != null);
        }

        private static CvNode FindChildNode(CvNode node, string[] path)
        {
            if (path.Length == 0) { return null; }

            string currentLevel = path.FirstOrDefault();

            if (string.IsNullOrEmpty(currentLevel)) { return null; }

            CvNode foundNode = null;

            if (DescriptionMatches(node, currentLevel)) { foundNode = node; }
            else if (node.Nodes != null)
            {
                foreach (CvNode childNode in node.Nodes)
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

            string[] subPath = new string[path.Length - 1];
            Array.Copy(path, 1, subPath, 0, subPath.Length);
            return FindChildNode(foundNode, subPath);
        }

        private static bool DescriptionMatches(CvNode node, string description)
        {
            return node.Description != null && node.Description.Any(cvDescription => cvDescription.Text == description);
        }

        public static Cv FindCv(this VendorCv vendorCv, string keyword)
        {
            return vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keyword);
        }

        public static IEnumerable<Cv> FindCvs(this VendorCv vendorCv, string keyword)
        {
            return vendorCv.Cvs.Where(x => x.Keyword == keyword);
        }

        public static IEnumerable<Cv> GetCvsWithChanges(this VendorCv vendorCv)
        {
            return vendorCv.Cvs.Where(x => !string.IsNullOrEmpty(x.NewValue) && x.Value != x.NewValue).ToList();
        }

        public static uint GetUIntValue(this VendorCv vendorCv, string keyword)
        {
            if (vendorCv.Cvs == null) { return default; }

            IEnumerable<Cv> cvs = vendorCv.Cvs.Where(x => x.Keyword == keyword);
            List<byte> valueBytes = cvs.Select(cv => Convert.ToByte(cv.Value, CultureInfo.CurrentCulture)).ToList();
            return BitConverter.ToUInt32(valueBytes.ToArray(), 0);
        }

        public static byte GetByteValue(this VendorCv vendorCv, string keyword)
        {
            if (vendorCv.Cvs == null) { return default; }
            Cv cv = vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keyword);
            return cv != null ? Convert.ToByte(cv.Value, CultureInfo.CurrentCulture) : (byte)0;
        }

        public static ushort GetUShortValue(this VendorCv vendorCv, string keywordLow, string keywordHigh)
        {
            Cv cvLow = vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keywordLow);
            Cv cvHigh = vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keywordHigh);
            return BitConverter.ToUInt16(new[] { Convert.ToByte(cvLow?.Value, CultureInfo.CurrentCulture), Convert.ToByte(cvHigh?.Value, CultureInfo.CurrentCulture) }, 0);
        }

        public static ushort GetUShortValue(this VendorCv vendorCv, string keywordLow)
        {
            Cv cvLow = vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keywordLow);
            Cv cvHigh = vendorCv.Cvs[vendorCv.Cvs.ToList().IndexOf(cvLow) + 1];
            return BitConverter.ToUInt16(new[] { Convert.ToByte(cvLow?.Value, CultureInfo.CurrentCulture), Convert.ToByte(cvHigh?.Value, CultureInfo.CurrentCulture) }, 0);
        }

        public static void SetValue(this VendorCv vendorCv, byte value, string keyword)
        {
            Cv cv = vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keyword);
            if (cv != null)
            {
                cv.NewValue = value.ToString(CultureInfo.CurrentCulture);
            }
        }


        public static void SetValue(this VendorCv vendorCv, ushort value, string keywordLow, string keywordHigh)
        {
            Cv cvLow = vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keywordLow);
            Cv cvHigh = vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keywordHigh);
            byte[] valueBytes = BitConverter.GetBytes(value);
            cvLow.NewValue = valueBytes[0].ToString(CultureInfo.CurrentCulture);
            cvHigh.NewValue = valueBytes[1].ToString(CultureInfo.CurrentCulture);
        }

        public static void SetValue(this VendorCv vendorCv, ushort value, string keywordLow)
        {
            Cv cvLow = vendorCv.Cvs.FirstOrDefault(x => x.Keyword == keywordLow);
            Cv cvHigh = vendorCv.Cvs[vendorCv.Cvs.ToList().IndexOf(cvLow) + 1];
            byte[] valueBytes = BitConverter.GetBytes(value);
            cvLow.NewValue = valueBytes[0].ToString(CultureInfo.CurrentCulture);
            cvHigh.NewValue = valueBytes[1].ToString(CultureInfo.CurrentCulture);
        }

        public static void SetValue(this VendorCv vendorCv, uint value, string keyword)
        {
            List<Cv> cvs = vendorCv.Cvs.Where(x => x.Keyword == keyword).ToList();
            byte[] valueBytes = BitConverter.GetBytes(value);
            if (cvs.Count != valueBytes.Length || cvs.Count != 4)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
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
    }
}