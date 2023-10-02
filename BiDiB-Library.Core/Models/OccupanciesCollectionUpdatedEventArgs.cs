using System;
using org.bidib.Net.Core.Models.BiDiB;

namespace org.bidib.Net.Core.Models;

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