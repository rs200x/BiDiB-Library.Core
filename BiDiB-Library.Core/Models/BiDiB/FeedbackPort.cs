using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
    public class FeedbackPort : ModelBase, IOccupanciesHost
    {
        private bool isFree;
        private OccupancyInfo[] occupancies;
        private ushort timeOccupied;
        private string name;

        [XmlAttribute("number")]
        public int Number { get; set; }

        [XmlAttribute("name")]
        public string Name
        {
            get => name;
            set => Set(() => Name, ref name, value);
        }

        [XmlIgnore]
        public bool IsFree
        {
            get => isFree;
            set { Set(() => IsFree, ref isFree, value); }
        }

        [XmlIgnore]
        public ushort TimeOccupied
        {
            get => timeOccupied;
            set => Set(() => TimeOccupied, ref timeOccupied, value);
        }

        [XmlIgnore]
        public OccupancyInfo[] Occupancies
        {
            get => occupancies;
            protected set { Set(() => Occupancies, ref occupancies, value); }
        }

        [XmlIgnore]
        public bool IsOccupied => Occupancies != null && Occupancies.Length > 0;

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? $"Port {Number}" : $"{Name} ({Number})";
        }

        #region IOccupancyProvider

        public OccupancyInfo GetOccupancy(ushort address)
        {
            return Occupancies?.FirstOrDefault(x => x.Address == address);
        }

        public ICollection<OccupancyInfo> GetOccupanciesByFilter(Func<OccupancyInfo, bool> filter)
        {
            return Occupancies?.Where(filter).ToList();
        }

        public virtual void AddOccupancy(OccupancyInfo occupancy)
        {
            if (occupancy == null) { return; }
            var currentOccupancies = Occupancies?.ToList() ?? new List<OccupancyInfo>();
            currentOccupancies.Add(occupancy);
            Occupancies = currentOccupancies.ToArray();
            RaisePropertyChanged(() => IsOccupied);
        }

        public void RemoveOccupancy(OccupancyInfo occupancy)
        {
            if (occupancy == null) { return; }
            var currentOccupancies = Occupancies?.ToList() ?? new List<OccupancyInfo>();
            currentOccupancies.Remove(occupancy);
            Occupancies = currentOccupancies.ToArray();
            RaisePropertyChanged(() => IsOccupied);
            occupancy.Dispose();
        }

        public void ClearOccupancies()
        {
            if (Occupancies != null)
            {
                foreach (var occupancy in Occupancies)
                {
                    occupancy.Dispose();
                }

                Occupancies = null;
            }

            RaisePropertyChanged(() => IsOccupied);
        }

        #endregion
    }
}