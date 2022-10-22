using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.core.Models.Firmware;

namespace org.bidib.netbidibc.core.Models.Helpers
{
    /// <summary>
    /// Extension class for restructuring of firmware data
    /// </summary>
    public static class FirmwareExtensions
    {
        private static readonly ILogger<FirmwareProtocol> logger = NullLoggerFactory.Instance.CreateLogger<FirmwareProtocol>();
        #region restructure

        public static void Restructure(this Firmware.Firmware firmware)
        {
            if (firmware == null) { throw new ArgumentNullException(nameof(firmware)); }
            if (firmware.Protocols == null) { return; }

            foreach (FirmwareProtocol protocol in firmware.Protocols)
            {
                RestructureProtocol(protocol);
            }
        }

        private static void RestructureProtocol(FirmwareProtocol protocol)
        {
            ApplyGroupDescriptions(protocol);

            if (protocol.CvStructure == null) { return; }
            foreach (Category category in protocol.CvStructure)
            {
                RestructureCategory(category, protocol.CVs);
            }
        }

        private static void ApplyGroupDescriptions(FirmwareProtocol protocol)
        {
            if (protocol?.CVs == null) { return; }

            foreach (CvGroup cvGroup in protocol.CVs.OfType<CvGroup>())
            {
                if (cvGroup.Cvs == null) { continue; }

                foreach (Cv cv in cvGroup.Cvs)
                {
                    cv.GroupDescriptions = cvGroup.Descriptions;
                }
            }
        }

        private static void RestructureCategory(Category category, CvBase[] cvItems)
        {
            foreach (Category subCategory in category.Items.OfType<Category>())
            {
                RestructureCategory(subCategory, cvItems);
            }

            if (cvItems == null) { return; }

            List<CvReference> references = category.Items.OfType<CvReference>().ToList();

            foreach (CvReference cvReference in references)
            {
                cvReference.CvItem = cvItems.FirstOrDefault(x => x.Id.Equals(cvReference.Id, StringComparison.OrdinalIgnoreCase));
            }
        }

        #endregion

        #region validation

        public static void Validate(this Firmware.Firmware firmware)
        {
            if (firmware == null) { throw new ArgumentNullException(nameof(firmware)); }
            if (firmware.Protocols == null) { return; }

            foreach (FirmwareProtocol protocol in firmware.Protocols)
            {
                ValidateProtocol(protocol, $"{firmware.ManufacturerId}-{firmware.ManufacturerExtendedId} -> {firmware.FullVersionString}");
            }
        }

        private static void ValidateProtocol(FirmwareProtocol protocol, string firmwareName)
        {
            if (protocol.CVs == null) { return; }
            List<CvBase> allItems = protocol.CVs.ToList();

            List<Cv> cvsInGroups = new List<Cv>();
            foreach (List<Cv> cvItems in allItems.OfType<CvGroup>().Select(x => x.Cvs.ToList()))
            {
                cvsInGroups.AddRange(cvItems);
            }

            List<Cv> cvs = allItems.OfType<Cv>().ToList();
            foreach (IGrouping<string, CvBase> multipleCvs in allItems.GroupBy(x => x.Id))
            {
                if (multipleCvs.Count() > 1)
                {
                    logger.LogWarning($"FW {firmwareName}: Cv with id '{multipleCvs.Key}' is defined more than once!");
                }
            }

            foreach (Cv cv in cvs.Where(x => cvsInGroups.Any(cg => x.Id == cg.Id)))
            {
                logger.LogWarning($"FW {firmwareName}: Cv with Id '{cv.Id}/{cv.FullNumber}' is also defined within a CvGroup!");
            }

            foreach (Cv cv in cvs.Where(x => x.Type == CvType.Select && x.Items == null))
            {
                logger.LogWarning($"FW { firmwareName}: Cv with Id '{cv.Id}/{cv.FullNumber}' is of type 'Select' but has no items defined!");
            }

            if (protocol.CvStructure == null) { return; }
            List<Cv> cvsInCategories = new List<Cv>();
            List<CvGroup> cvGroupsInCategories = new List<CvGroup>();
            foreach (Category category in protocol.CvStructure)
            {
                ValidateCategory(category, protocol.CVs.OfType<CvGroup>().ToList(), firmwareName, ref cvsInCategories, ref cvGroupsInCategories);
            }

            foreach (Cv cv in cvs.Where(x => cvsInCategories.All(c => c.Id != x.Id)))
            {
                logger.LogWarning($"FW {firmwareName}: Cv with id '{cv.Id}/{cv.FullNumber}' is not referenced in any category!");
            }

            foreach (CvGroup cvGroup in allItems.OfType<CvGroup>().Where(x => cvGroupsInCategories.All(g => !g.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase))))
            {
                logger.LogWarning($"FW {firmwareName}: CvGroup with id '{cvGroup.Id}' is not referenced in any category!");
            }
        }

        private static void ValidateCategory(Category category, List<CvGroup> cvGroupItems, string firmwareName, ref List<Cv> cvsInCategories, ref List<CvGroup> cvGroupsInCategories)
        {
            foreach (Category subCategory in category.Items.OfType<Category>())
            {
                ValidateCategory(subCategory, cvGroupItems, firmwareName, ref cvsInCategories, ref cvGroupsInCategories);
            }

            List<object> allItems = category.Items.ToList();
            List<CvReference> cvReferences = category.Items.OfType<CvReference>().ToList();

            foreach (CvReference cvReference in cvReferences.Where(x => x.CvItem == null))
            {
                logger.LogWarning(cvGroupItems.Any(x => x.Cvs.Any(c => c.Id == cvReference.Id))
                    ? $"FW {firmwareName}: CvReference with id '{cvReference.Id}' points to a CV which is defined within a CvGroup!"
                    : $"FW {firmwareName}: CvReference with id '{cvReference.Id}' points to a not defined CV!");

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
            List<CvBase> duplicatedItems = new List<CvBase>();
            foreach (IGrouping<string, Cv> cvs in cvsGrouped.Where(x=>x.Count() > 1))
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
                logger.LogDebug($"{duplicatedItems.Count} duplicated cv items removed from firmware protocol '{firmwareVersion}-{protocol.Type}'");
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
}
