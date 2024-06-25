using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.VendorCv;

public class VendorCvConverter
{
    private Dictionary<string, Cv> cvDictionary;

    public void ConvertToV2Structure(VendorCv vendorCv)
    {
        if (vendorCv == null)
        {
            return;
        }
        
        if (vendorCv.CvDefinition == null)
        {
            vendorCv.Cvs = Array.Empty<Cv>();
            return;
        }

        cvDictionary = new Dictionary<string, Cv>();
        foreach (var cvNode in vendorCv.CvDefinition)
        {
            RestructureCvNode(cvNode, vendorCv.Templates);
        }

        vendorCv.Cvs = cvDictionary.Values.ToArray();
        cvDictionary = null;
    }

    private void RestructureCvNode(CvNode cvNode, ICollection<Template> templates, int nodeIndex = 0)
    {
        var cvs = new List<Cv>();
        var cvReferences = new List<CvReference>();
        var nodes = new List<CvNode>();

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

        foreach (var cv in cvs)
        {
            if (cv.Type == CvDataType.Bit) { ConvertBitCv(cv); }
            if (cv.Type == CvDataType.Radio) { ConvertRadioCv(cv); }

            var cvReference = cvReferences.Find(x => cv.Type == CvDataType.Bit && x.Number == cv.Number);

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
        var template = templates.FirstOrDefault(x => x.Name == cvNode.Template).Clone();

        if (template?.Items == null) { return; }

        GenerateNodesForCvItems(cvNode, template, nodeIndex, cvs, cvReferences);

        foreach (var generatedCv in template.Items.OfType<Repeater>().Select(r => ProcessRepeater(r, cvNode.Offset)).SelectMany(cv => cv))
        {
            AddCv(generatedCv, cvs, cvReferences);
        }

        foreach (var node in template.Items.OfType<CvNode>())
        {
            node.Offset = cvNode.Offset;
            RestructureCvNode(node, templates);
            nodes.Add(node);
        }
    }

    private void GenerateNodesForCvItems(CvNode cvNode, Template template, int nodeIndex, ICollection<Cv> cvs, ICollection<CvReference> cvReferences)
    {
        foreach (var cv in template.Items.OfType<Cv>())
        {
            cv.Number = (Convert.ToInt32(cv.Number, CultureInfo.InvariantCulture) + cvNode.Offset).ToString(CultureInfo.InvariantCulture);
            AddCv(cv, cvs, cvReferences);

            if (!string.IsNullOrEmpty(cv.Keyword))
            {
                cv.Keyword = cv.Keyword.Replace("%%d", nodeIndex.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
                cv.Keyword = cv.Keyword.Replace("%%p", (nodeIndex + 1).ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
            }

            if (cv.Type == CvDataType.Long)
            {
                for (var i = 1; i < 4; i++)
                {
                    var subCv = cv.Clone();
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
    }

    private void GenerateNodesForItems(CvNode cvNode, ICollection<Template> templates, ICollection<Cv> cvs, List<CvReference> cvReferences, List<CvNode> nodes)
    {
        if (cvNode.Items == null) { return; }

        foreach (var cv in cvNode.Items.OfType<Cv>())
        {
            cv.Number = (Convert.ToInt32(cv.Number, CultureInfo.InvariantCulture) + cvNode.Offset).ToString(CultureInfo.InvariantCulture);
            AddCv(cv, cvs, cvReferences);
        }
        
        ProcessRepeaterReferences(cvNode, cvs, cvReferences);

        foreach (var cvReference in cvNode.Items.OfType<CvReference>())
        {
            cvReference.Number = (Convert.ToInt32(cvReference.Number, CultureInfo.InvariantCulture) + cvNode.Offset).ToString(CultureInfo.InvariantCulture);
            cvReferences.Add(cvReference);
        }

        var cvNodes = cvNode.Items.OfType<CvNode>();
        foreach (var node in cvNodes)
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

    private void ProcessRepeaterReferences(CvNode cvNode, ICollection<Cv> cvs, List<CvReference> cvReferences)
    {
        foreach (var repeater in cvNode.Items.OfType<Repeater>())
        {
            if (repeater.Cvs != null)
            {
                var repeaterCvs = ProcessRepeater(repeater, cvNode.Offset);
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
    }

    private IEnumerable<CvNode> GenerateDuplicatedNodes(CvNode cvNode, ICollection<Template> templates)
    {
        var nodes = new List<CvNode>();

        var offset = cvNode.Offset;

        for (var i = 0; i < cvNode.Count; i++)
        {
            var newNode = cvNode.Clone();
            newNode.Count = 0;
            newNode.Next = 0;
            newNode.Offset = offset;

            RestructureCvNode(newNode, templates, i);

            foreach (var description in newNode.Description)
            {
                description.Text = description.Text.Replace("%%d", i.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
                description.Text = description.Text.Replace("%%p", (i + 1).ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
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

        var bitDescriptions = new List<CvBitDescription>();
        foreach (var bitGroup in bitGroups)
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
        if (string.IsNullOrEmpty(values) || values == "-" || values.Contains(',')) { return values; }

        if (values.Contains(';'))
        {
            return values.Replace(";", ",", StringComparison.InvariantCultureIgnoreCase);
        }

        var valuesByte = Convert.ToByte(values, CultureInfo.InvariantCulture);
        var vArray = new BitArray(new[] { valuesByte });

        var bitNumbers = new List<int>();
        for (var i = 0; i < vArray.Length; i++)
        {
            if (vArray.Get(i))
            {
                bitNumbers.Add(i);
            }
        }

        return string.Join(",", bitNumbers.Select(x => x.ToString(CultureInfo.InvariantCulture)));
    }

    private static void ConvertRadioCv(Cv cv)
    {
        var bitGroups = cv.BitDescription.GroupBy(x => x.Bitnum);

        var radioDescriptions = new List<CvRadioDescription>();

        var separatedValues = !string.IsNullOrEmpty(cv.Values) && cv.Values != "-"
            ? cv.Values.Split(',')
            : Array.Empty<string>();

        var values = !string.IsNullOrEmpty(cv.RadioValues)
            ? cv.RadioValues.Split(',')
            : separatedValues;

        var groups = !string.IsNullOrEmpty(cv.RadioGroups) ? cv.RadioGroups.Split(';') : Array.Empty<string>();

        var groupItems = groups.Any() ? groups[0].Split(',') : Array.Empty<string>();

        var index = 0;

        foreach (var bitGroup in bitGroups)
        {
            var value = index < values.Length ? Convert.ToByte(values[index], CultureInfo.InvariantCulture) : (byte)index;
            var group = groupItems.Any() && groupItems.Contains(bitGroup.Key.ToString(CultureInfo.InvariantCulture)) ? CvRadioDescription.DefaultGroupName : string.Empty;

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
        cvDictionary.TryAdd(cv.Number, cv);
        
        cvs.Add(cv);
        cvReferences.Add(new CvReference { Number = cv.Number, Values = cv.Values });
    }


    private static IEnumerable<Cv> ProcessRepeater(Repeater repeater, int baseOffset)
    {
        var cvs = new List<Cv>();
        var offset = repeater.Offset + baseOffset;
        for (var i = 0; i < repeater.Count; i++)
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

        foreach (var description in cv.Description)
        {
            description.Text = description.Text.Replace("%%d", (index + 1).ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
        }

        return cv;
    }

    private static IEnumerable<CvReference> ProcessRepeaterWithCvRefs(Repeater repeater)
    {
        var cvReferences = new List<CvReference>();
        var offset = repeater.Offset;
        for (var i = 0; i < repeater.Count; i++)
        {
            foreach (var cvReference in repeater.CvReferences)
            {
                var cv = cvReference.Clone();
                cv.Number = (Convert.ToInt32(cvReference.Number, CultureInfo.InvariantCulture) + offset).ToString(CultureInfo.InvariantCulture);
                cvReferences.Add(cv);
            }

            offset += repeater.Next > 0 ? repeater.Next : 1;
        }

        return cvReferences;
    }
}