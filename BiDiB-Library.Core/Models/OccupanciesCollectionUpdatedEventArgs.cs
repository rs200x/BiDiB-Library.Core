using System;
using org.bidib.netbidibc.core.Models.BiDiB;

namespace org.bidib.netbidibc.core.Models
{
    public class OccupanciesCollectionUpdatedEventArgs : EventArgs
    {
        public OccupanciesCollectionUpdatedEventArgs(OccupancyInfo addedItem, OccupancyInfo removeItem)
        {
            AddedItem = addedItem;
            RemoveItem = removeItem;
        }

        public OccupancyInfo AddedItem { get; }

        public OccupancyInfo RemoveItem { get; }
    }
}