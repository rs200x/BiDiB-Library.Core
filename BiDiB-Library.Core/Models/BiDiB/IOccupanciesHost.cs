using System;
using System.Collections.Generic;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    public interface IOccupanciesHost
    {
        OccupancyInfo GetOccupancy(ushort address);

        ICollection<OccupancyInfo> GetOccupanciesByFilter(Func<OccupancyInfo, bool> filter);

        void AddOccupancy(OccupancyInfo occupancy);

        void RemoveOccupancy(OccupancyInfo occupancy);

        void ClearOccupancies();
    }
}