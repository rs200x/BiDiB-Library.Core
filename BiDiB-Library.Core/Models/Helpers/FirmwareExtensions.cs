using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Core.Models.Firmware;

namespace org.bidib.Net.Core.Models.Helpers;

/// <summary>
/// Extension class for restructuring of firmware data
/// </summary>
public static class FirmwareExtensions
{
    private static readonly ILogger<FirmwareProtocol> Logger = NullLoggerFactory.Instance.CreateLogger<FirmwareProtocol>();
    #region restructure

    public static void Restructure(this Firmware.Firmware firmware)
    {
        if (firmware == null) { throw new ArgumentNullException(nameof(firmware)); }
        if (firmware.Protocols == null) { return; }

        foreach (var protocol in firmware.Protocols)
        {
            RestructureProtocol(protocol);
        }
    }

    private static void RestructureProtocol(FirmwareProtocol protocol)
    {
        ApplyGroupDescriptions(protocol);

        if (protocol.CvStructure == null) { return; }
        foreach (var category in protocol.CvStructure)
        {
            RestructureCategory(category, protocol.CVs);
        }
    }

    private static void ApplyGroupDescriptions(FirmwareProtocol protocol)
    {
        if (protocol?.CVs == null) { return; }

        foreach (var cvGroup in protocol.CVs.OfType<CvGroup>())
        {
            if (cvGroup.Cvs == null) { continue; }

            foreach (var cv in cvGroup.Cvs)
            {
                cv.GroupDescriptions = cvGroup.Descriptions;
            }
        }
    }

    private static void RestructureCategory(Category category, CvBase[] cvItems)
    {
        foreach (var subCategory in category.Items.OfType<Category>())
        {
            RestructureCategory(subCategory, cvItems);
        }

        if (cvItems == null) { return; }

        var references = category.Items.OfType<CvReference>().ToList();

        foreach (var cvReference in references)
        {
            cvReference.CvItem = Array.Find(cvItems,x => x.Id.Equals(cvReference.Id, StringComparison.OrdinalIgnoreCase));
        }
    }

    #endregion

    #region validation

    public static void Validate(this Firmware.Firmware firmware)
    {
        if (firmware == null) { throw new ArgumentNullException(nameof(firmware)); }
        if (firmware.Protocols == null) { return; }

        foreach (var protocol in firmware.Protocols)
        {
            ValidateProtocol(protocol, $"{firmware.ManufacturerId}-{firmware.ManufacturerExtendedId} -> {firmware.FullVersionString}");
        }
    }

    private static void ValidateProtocol(FirmwareProtocol protocol, string firmwareName)
    {
        if (protocol.CVs == null) { return; }
        var allItems = protocol.CVs.ToList();

        var cvsInGroups = new List<Cv>();
        foreach (var cvItems in allItems.OfType<CvGroup>().Select(x => x.Cvs.ToList()))
        {
            cvsInGroups.AddRange(cvItems);
        }

        var cvs = allItems.OfType<Cv>().ToList();
        foreach (var multipleCvs in allItems.GroupBy(x => x.Id))
        {
            if (multipleCvs.Count() > 1)
            {
                Logger.LogWarning("FW {FirmwareName}: Cv with id '{Id}' is defined more than once!", firmwareName, multipleCvs.Key);
            }
        }

        foreach (var cv in cvs.Where(x => cvsInGroups.Exists(cg => x.Id == cg.Id)))
        {
            Logger.LogWarning("FW {FirmwareName}: Cv with Id '{Id}/{Number}' is also defined within a CvGroup!", firmwareName, cv.Id, cv.FullNumber);
        }

        foreach (var cv in cvs.Where(x => x.Type == CvType.Select && x.Items == null))
        {
            Logger.LogWarning("FW {FirmwareName}: Cv with Id '{Id}/{Number}' is of type 'Select' but has no items defined!", firmwareName, cv.Id, cv.FullNumber);
        }

        if (protocol.CvStructure == null) { return; }
        var cvsInCategories = new List<Cv>();
        var cvGroupsInCategories = new List<CvGroup>();
        foreach (var category in protocol.CvStructure)
        {
            ValidateCategory(category, protocol.CVs.OfType<CvGroup>().ToList(), firmwareName, ref cvsInCategories, ref cvGroupsInCategories);
        }

        foreach (var cv in cvs.Where(x => cvsInCategories.TrueForAll(c => c.Id != x.Id)))
        {
            Logger.LogWarning("FW {FirmwareName}: Cv with id '{Id}/{Number}' is not referenced in any category!", firmwareName, cv.Id, cv.FullNumber);
        }

        foreach (var cvGroup in allItems.OfType<CvGroup>().Where(x => cvGroupsInCategories.TrueForAll(g => !g.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase))))
        {
            Logger.LogWarning("FW {FirmwareName}: CvGroup with id '{CvGroupId}' is not referenced in any category!", firmwareName, cvGroup.Id);
        }
    }

    private static void ValidateCategory(Category category, List<CvGroup> cvGroupItems, string firmwareName, ref List<Cv> cvsInCategories, ref List<CvGroup> cvGroupsInCategories)
    {
        foreach (var subCategory in category.Items.OfType<Category>())
        {
            ValidateCategory(subCategory, cvGroupItems, firmwareName, ref cvsInCategories, ref cvGroupsInCategories);
        }

        var allItems = category.Items.ToList();
        var cvReferences = category.Items.OfType<CvReference>().ToList();

        foreach (var cvReference in cvReferences.Where(x => x.CvItem == null))
        {
            Logger.LogWarning(cvGroupItems.Exists(x => Array.Exists( x.Cvs, c => c.Id == cvReference.Id))
                ? "FW {firmwareName}: CvReference with id '{Id}' points to a CV which is defined within a CvGroup!"
                : "FW {firmwareName}: CvReference with id '{Id}' points to a not defined CV!", firmwareName, cvReference.Id);

            allItems.Remove(cvReference);
        }

        cvsInCategories.AddRange(cvReferences.Where(x => x.CvItem != null).Select(x => x.CvItem).OfType<Cv>());
        cvGroupsInCategories.AddRange(cvReferences.Where(x => x.CvItem != null).Select(x => x.CvItem).OfType<CvGroup>());

        category.Items = allItems.ToArray();
    }

    #endregion

    public static void FilterDuplicated(this Firmware.Firmware firmware)
    {
        foreach (var protocol in firmware.Protocols)
        {
            FilterDuplicated(protocol, firmware.FullVersionString);
        }
    }

    private static void FilterDuplicated(FirmwareProtocol protocol, string firmwareVersion)
    {
        if (protocol.CVs == null) { return; }

        var allCvs = protocol.CVs.ToList();
        var cvsGrouped = allCvs.OfType<Cv>().GroupBy(x => x.FullNumber);
        var duplicatedItems = new List<CvBase>();
        foreach (var cvs in cvsGrouped.Where(x=>x.Count() > 1))
        {
            duplicatedItems.AddRange(cvs.Skip(1));
        }

        var groupsGroups = allCvs.OfType<CvGroup>().GroupBy(GetGroupNumber);
        foreach (var groups in groupsGroups)
        {
            if (groups.Count() > 1)
            {
                duplicatedItems.AddRange(groups.Skip(1));
            }

            var firstGroup = groups.ElementAt(0);
            var groupCvs = firstGroup.Cvs.ToList();
            var subCvsGrouped = groupCvs.GroupBy(x => x.FullNumber);
            List<Cv> duplicatedGroupItems = new ();
            foreach (var cvs in subCvsGrouped.Where(x => x.Count() > 1))
            {
                duplicatedGroupItems.AddRange(cvs.Skip(1));
            }

            duplicatedItems.AddRange(duplicatedGroupItems);
            duplicatedGroupItems.ForEach(x => groupCvs.Remove(x));
            firstGroup.Cvs = groupCvs.ToArray();
        }

        duplicatedItems.ForEach(x=> allCvs.Remove(x));
        protocol.CVs = allCvs.ToArray();
        RemoveReferencesOfDuplicates(protocol.CvStructure, duplicatedItems);

        if (duplicatedItems.Any())
        {
            Logger.LogDebug("{DuplicatedItems} duplicated cv items removed from firmware protocol '{FirmwareVersion}-{ProtocolType}'", duplicatedItems.Count,firmwareVersion, protocol.Type);
        }
    }

    private static string GetGroupNumber(CvGroup cvGroup)
    {
        return string.Join(",", cvGroup.Cvs.OrderBy(x => x.FullNumber).Select(x => x.FullNumber));
    }

    private static void RemoveReferencesOfDuplicates(IEnumerable<Category> categories, ICollection<CvBase> duplicated)
    {
        if(categories == null) {return;}

        foreach (var category in categories)
        {
            var items = category.Items.ToList();
            items.RemoveAll(x => x is CvReference cvReference && duplicated.Contains(cvReference.CvItem));
            category.Items = items.ToArray();

            RemoveReferencesOfDuplicates(items.OfType<Category>(), duplicated);
        }
    }
}