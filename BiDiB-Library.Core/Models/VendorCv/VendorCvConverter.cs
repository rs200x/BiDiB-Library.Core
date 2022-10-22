using org.bidib.netbidibc.core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace org.bidib.netbidibc.core.Models.VendorCv
{
    public class VendorCvConverter
    {
        private Dictionary<string, Cv> cvDictionary;

        public void ConvertToV2Structure(VendorCv vendorCv)
        {
            if (vendorCv.CvDefinition == null)
            {
                vendorCv.Cvs = new Cv[0];
                return;
            }

            cvDictionary = new Dictionary<string, Cv>();
            foreach (CvNode cvNode in vendorCv.CvDefinition)
            {
                RestructureCvNode(cvNode, vendorCv.Templates);
            }

            vendorCv.Cvs = cvDictionary.Values.ToArray();
            cvDictionary = null;
        }

        private void RestructureCvNode(CvNode cvNode, ICollection<Template> templates, int nodeIndex = 0)
        {
            List<Cv> cvs = new List<Cv>();
            List<CvReference> cvReferences = new List<CvReference>();
            List<CvNode> nodes = new List<CvNode>();

            if (cvNode.Count > 1)
            {
                nodes.AddRange(GenerateDuplicatedNodes(cvNode, templates));
            }
            else
            {
                GenerateNodesForItems(cvNode, templates, cvs, cvReferences, nodes);

                if (!string.IsNullOrEmpty(cvNode.Template))
                {
                    GenerateNodesForTemplate(cvNode, templates, nodeIndex, cvs, cvReferences, nodes);
                }
            }

            if (cvNode.NodeText != null)
            {
                cvNode.Description = cvNode.NodeText.Select(cvNodeText => new CvDescription { Lang = cvNodeText.Lang, Help = cvNodeText.Help, Text = cvNodeText.Text }).ToArray();
            }

            foreach (Cv cv in cvs)
            {
                if (cv.Type == CvDataType.Bit) { ConvertBitCv(cv); }
                if (cv.Type == CvDataType.Radio) { ConvertRadioCv(cv); }

                CvReference cvReference = cvReferences.FirstOrDefault(x => cv.Type == CvDataType.Bit && x.Number == cv.Number);

                if (cvReference != null)
                {
                    cvReference.Values = ConvertToList(cvReference.Values);
                }
            }

            cvNode.CVs = cvs.ToArray();
            cvNode.CvReferences = cvReferences.ToArray();
            cvNode.Nodes = nodes.ToArray();
        }

        private void GenerateNodesForTemplate(CvNode cvNode, ICollection<Template> templates, int nodeIndex, ICollection<Cv> cvs, ICollection<CvReference> cvReferences, ICollection<CvNode> nodes)
        {
            Template template = templates.FirstOrDefault(x => x.Name == cvNode.Template).Clone();

            if (template?.Items == null) { return; }

            foreach (Cv cv in template.Items.OfType<Cv>())
            {
                cv.Number = (Convert.ToInt32(cv.Number, CultureInfo.InvariantCulture) + cvNode.Offset).ToString(CultureInfo.InvariantCulture);
                AddCv(cv, cvs, cvReferences);

                if (!string.IsNullOrEmpty(cv.Keyword))
                {
                    cv.Keyword = cv.Keyword.Replace("%%d", nodeIndex.ToString(CultureInfo.InvariantCulture));
                    cv.Keyword = cv.Keyword.Replace("%%p", (nodeIndex + 1).ToString(CultureInfo.InvariantCulture));
                }

                if (cv.Type == CvDataType.Long)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        Cv subCv = cv.Clone();
                        subCv.Number = (Convert.ToInt32(cv.Number, CultureInfo.InvariantCulture) + i).ToString(CultureInfo.InvariantCulture);
                        AddCv(subCv, cvs, cvReferences);
                    }
                }

                cv.High = !string.IsNullOrEmpty(cv.High) && cv.High != "-"
                    ? (Convert.ToInt32(cv.High, CultureInfo.InvariantCulture) + cvNode.Offset).ToString(CultureInfo.InvariantCulture)
                    : cv.High;

                cv.Low = !string.IsNullOrEmpty(cv.Low) && cv.Low != "-"
                    ? (Convert.ToInt32(cv.Low, CultureInfo.InvariantCulture) + cvNode.Offset).ToString(CultureInfo.InvariantCulture)
                    : cv.Low;
            }

            foreach (Cv generatedCv in template.Items.OfType<Repeater>().Select(ProcessRepeater).SelectMany(cv => cv))
            {
                AddCv(generatedCv, cvs, cvReferences);
            }

            foreach (CvNode node in template.Items.OfType<CvNode>())
            {
                node.Offset = cvNode.Offset;
                RestructureCvNode(node, templates);
                nodes.Add(node);
            }
        }

        private void GenerateNodesForItems(CvNode cvNode, ICollection<Template> templates, ICollection<Cv> cvs, List<CvReference> cvReferences, List<CvNode> nodes)
        {
            if (cvNode.Items == null) { return; }

            foreach (Cv cv in cvNode.Items.OfType<Cv>())
            {
                cv.Number = (Convert.ToInt32(cv.Number, CultureInfo.InvariantCulture) + cvNode.Offset).ToString(CultureInfo.InvariantCulture);
                AddCv(cv, cvs, cvReferences);
            }

            foreach (Repeater repeater in cvNode.Items.OfType<Repeater>())
            {
                if (repeater.Cvs != null)
                {
                    var repeaterCvs = ProcessRepeater(repeater);
                    foreach (var cv in repeaterCvs)
                    {
                        AddCv(cv,cvs,cvReferences);
                    }
                }

                if (repeater.CvReferences != null)
                {
                    cvReferences.AddRange(ProcessRepeaterWithCvRefs(repeater));
                }
            }

            foreach (CvReference cvReference in cvNode.Items.OfType<CvReference>())
            {
                cvReference.Number = (Convert.ToInt32(cvReference.Number, CultureInfo.InvariantCulture) + cvNode.Offset).ToString(CultureInfo.InvariantCulture);
                cvReferences.Add(cvReference);
            }

            IEnumerable<CvNode> cvNodes = cvNode.Items.OfType<CvNode>();
            foreach (CvNode node in cvNodes)
            {
                RestructureCvNode(node, templates);
                if (!string.IsNullOrEmpty(node.Template) && node.Nodes?.Length > 0)
                {
                    nodes.AddRange(node.Nodes);
                }
                else
                {
                    nodes.Add(node);
                }
            }
        }

        private IEnumerable<CvNode> GenerateDuplicatedNodes(CvNode cvNode, ICollection<Template> templates)
        {
            var nodes = new List<CvNode>();

            int offset = cvNode.Offset;

            for (int i = 0; i < cvNode.Count; i++)
            {
                CvNode newNode = cvNode.Clone();
                newNode.Count = 0;
                newNode.Next = 0;
                newNode.Offset = offset;

                RestructureCvNode(newNode, templates, i);

                foreach (CvDescription description in newNode.Description)
                {
                    description.Text = description.Text.Replace("%%d", i.ToString(CultureInfo.InvariantCulture));
                    description.Text = description.Text.Replace("%%p", (i + 1).ToString(CultureInfo.InvariantCulture));
                }

                nodes.Add(newNode);
                offset += cvNode.Next;
            }

            return nodes;
        }

        private static void ConvertBitCv(Cv cv)
        {
            if (cv.BitDescription == null) { return; }
            var bitGroups = cv.BitDescription.GroupBy(x => x.Bitnum);

            List<CvBitDescription> bitDescriptions = new List<CvBitDescription>();
            foreach (IGrouping<int, CvBitDescription> bitGroup in bitGroups)
            {
                var bitDescription = new CvBitDescription
                {
                    Number = bitGroup.Key - 1,
                    Description = bitGroup.Select(bd => new CvDescription { Text = bd.Text, Help = bd.Help, Lang = bd.Lang }).ToArray()
                };

                bitDescriptions.Add(bitDescription);
            }

            cv.Bit = bitDescriptions.ToArray();
        }

        private static string ConvertToList(string values)
        {
            if (string.IsNullOrEmpty(values) || values == "-" || values.Contains(",")) { return values; }

            if (values.Contains(";"))
            {
                return values.Replace(";", ",");
            }

            byte valuesByte = Convert.ToByte(values, CultureInfo.InvariantCulture);
            BitArray vArray = new BitArray(new[] { valuesByte });

            List<int> bitNumbers = new List<int>();
            for (int i = 0; i < vArray.Length; i++)
            {
                if (vArray.Get(i))
                {
                    bitNumbers.Add(i);
                }
            }

            return string.Join(",", bitNumbers.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        }

        private void ConvertRadioCv(Cv cv)
        {
            var bitGroups = cv.BitDescription.GroupBy(x => x.Bitnum);

            List<CvRadioDescription> radioDescriptions = new List<CvRadioDescription>();

            var separatedValues = !string.IsNullOrEmpty(cv.Values) && cv.Values != "-"
                ? cv.Values.Split(',')
                : new string[] { };

            string[] values = !string.IsNullOrEmpty(cv.RadioValues)
                ? cv.RadioValues.Split(',')
                : separatedValues;

            string[] groups = !string.IsNullOrEmpty(cv.RadioGroups) ? cv.RadioGroups.Split(';') : new string[] { };

            string[] groupItems = groups.Any() ? groups[0].Split(',') : new string[] { };

            int index = 0;

            foreach (IGrouping<int, CvBitDescription> bitGroup in bitGroups)
            {
                byte value = index < values.Length ? Convert.ToByte(values[index], CultureInfo.InvariantCulture) : (byte)index;
                string group = groupItems.Any() && groupItems.Contains(bitGroup.Key.ToString(CultureInfo.InvariantCulture)) ? CvRadioDescription.DefaultGroupName : string.Empty;

                var radioDescription = new CvRadioDescription
                {
                    Value = value,
                    Group = group,
                    Description = bitGroup.Select(bitDescription => new CvDescription { Text = bitDescription.Text, Help = bitDescription.Help, Lang = bitDescription.Lang }).ToArray()
                };

                radioDescriptions.Add(radioDescription);
                index++;
            }

            cv.Radio = radioDescriptions.ToArray();
        }

        private void AddCv(Cv cv, ICollection<Cv> cvs, ICollection<CvReference> cvReferences)
        {
            if (!cvDictionary.ContainsKey(cv.Number))
            {
                cvDictionary.Add(cv.Number, cv);
            }

            cvs.Add(cv);
            cvReferences.Add(new CvReference { Number = cv.Number, Values = cv.Values });
        }


        private static IEnumerable<Cv> ProcessRepeater(Repeater repeater)
        {
            List<Cv> cvs = new List<Cv>();
            int offset = repeater.Offset;
            for (int i = 0; i < repeater.Count; i++)
            {
                cvs.AddRange(repeater.Cvs.Select(cvTemplate => GetCvFromTemplate(cvTemplate, offset, i)));
                offset += repeater.Next > 0 ? repeater.Next : 1;
            }

            return cvs;
        }

        private static Cv GetCvFromTemplate(Cv cvTemplate, int offset, int index)
        {
            var cv = cvTemplate.Clone();
            cv.Number = (Convert.ToInt32(cvTemplate.Number, CultureInfo.InvariantCulture) + offset).ToString(CultureInfo.InvariantCulture);

            cv.High = !string.IsNullOrEmpty(cvTemplate.High) && cvTemplate.High != "-"
                ? (Convert.ToInt32(cvTemplate.High, CultureInfo.InvariantCulture) + offset).ToString(CultureInfo.InvariantCulture)
                : cvTemplate.High;

            cv.Low = !string.IsNullOrEmpty(cvTemplate.Low) && cvTemplate.Low != "-"
                ? (Convert.ToInt32(cvTemplate.Low, CultureInfo.InvariantCulture) + offset).ToString(CultureInfo.InvariantCulture)
                : cvTemplate.Low;

            if (cv.Description == null) { return cv; }

            foreach (CvDescription description in cv.Description)
            {
                description.Text = description.Text.Replace("%%d", (index + 1).ToString(CultureInfo.InvariantCulture));
            }

            return cv;
        }

        private static IEnumerable<CvReference> ProcessRepeaterWithCvRefs(Repeater repeater)
        {
            List<CvReference> cvReferences = new List<CvReference>();
            int offset = repeater.Offset;
            for (int i = 0; i < repeater.Count; i++)
            {
                foreach (CvReference cvReference in repeater.CvReferences)
                {
                    CvReference cv = cvReference.Clone();
                    cv.Number = (Convert.ToInt32(cvReference.Number, CultureInfo.InvariantCulture) + offset).ToString(CultureInfo.InvariantCulture);
                    cvReferences.Add(cv);
                }

                offset += repeater.Next > 0 ? repeater.Next : 1;
            }

            return cvReferences;
        }
    }
}