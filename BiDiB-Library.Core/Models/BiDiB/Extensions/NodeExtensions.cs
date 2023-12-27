using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.BiDiB.Extensions;

public static class NodeExtensions
{
    public static int GetAddress(this Node node) => node?.Address.GetArrayValue() ?? 0;

    public static int GetParentAddress(this Node node)
    {
        if (node == null) { return -1; }

        // child of root node
        if (node.Address.Length == 1) { return 0; }

        var parentAddressLength = node.Address.Length;

        // is interface node below root
        if (node.Address.Last() == 0)
        {
            parentAddressLength--;
        }

        var parentAddress = new byte[parentAddressLength];
        Array.Copy(node.Address, 0, parentAddress, 0, parentAddressLength);

        // interface address node always ends with 0
        parentAddress[parentAddressLength - 1] = 0;

        return parentAddress.GetArrayValue();
    }

    public static void SetUniqueId(this Node node, byte[] idBytes)
    {
        if (node == null || idBytes is not { Length: 7 }) { return; }

        var longBytes = new byte[8];
        idBytes.CopyTo(longBytes, 1);

        // If the system architecture is little endian (that is, little end first),
        // reverse the byte array.
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(longBytes);
        }

        node.UniqueId = BitConverter.ToInt64(longBytes, 0);
        node.UniqueIdBytes = idBytes;
    }

    internal static byte[] GetUniqueIdBytes(this Node node)
    {
        var longBytes = BitConverter.GetBytes(node.UniqueId);

        // If the system architecture is little-endian (that is, little end first),
        // reverse the byte array.
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(longBytes);
        }

        var size = longBytes.Length == 8 ? 1 : 0;

        var uniqueIdBytes = new byte[7];
        Array.Copy(longBytes, size, uniqueIdBytes, 0, 7);

        return uniqueIdBytes;
    }

    public static byte[] GetUniqueIdBytesWithoutClass(this Node node)
    {
        if (node == null) { return Array.Empty<byte>(); }

        var shortId = new byte[7];
        Array.Copy(node.UniqueIdBytes, 2, shortId, 2, 5);
        return shortId;
    }

    public static long GetUniqueIdWithoutClass(this Node node)
    {
        if (node == null) { return 0; }

        var shortIdBytes = GetUniqueIdBytesWithoutClass(node);
        var shortId = new byte[8];
        Array.Copy(shortIdBytes, 0, shortId, 1, 7);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(shortId);
        }
        return BitConverter.ToInt64(shortId, 0);
    }

    public static string GetFullAddressString(this Node node)
    {
        return GetFullAddressString(node?.Address);
    }

    public static string GetFullAddressString(byte[] address)
    {
        if (address == null) { return string.Empty; }

        StringBuilder sb = new();
        for (var index = 0; index < 4; index++)
        {
            if (index < address.Length)
            {
                sb.Append(address[index]);
            }
            else
            {
                sb.Append("0");
            }
            if (index < 3)
            {
                sb.Append(".");
            }
        }
        return sb.ToString();
    }

    public static void AddOrUpdateFeature(this Node node, Feature newFeature)
    {
        if (node == null || newFeature == null) { return; }

        var features = node.Features?.ToList() ?? new List<Feature>();
        var feature = features.FirstOrDefault(x => x.FeatureId == newFeature.FeatureId);

        if (feature != null)
        {
            feature.Value = newFeature.Value;
        }
        else
        {
            features.Add(newFeature);
        }

        node.Features = features.ToArray();
    }

    public static Feature GetFeature(this Node node, BiDiBFeature featureType)
    {
        if (node?.Features == null || !node.Features.Any()) { return null; }

        return node.Features.FirstOrDefault(x => x.FeatureType == featureType);
    }

    public static Feature GetFeature(this Node node, byte featureId)
    {
        if (node?.Features == null || !node.Features.Any()) { return null; }

        return Array.Find(node.Features, x => x.FeatureId == featureId);
    }

    public static bool IsFeatureActive(this Node node, BiDiBFeature featureType)
    {
        if (node?.Features == null || !node.Features.Any()) { return false; }

        var feature = Array.Find(node.Features, x => x.FeatureType == featureType);

        return feature is { Value: > 0 };
    }

    public static bool IsSubNodeOf(this Node node, Node parentNode)
    {
        if (node == null || parentNode == null) { return false; }

        // parent is root node => all other nodes are sub nodes
        if (parentNode.GetAddress() == 0 && node.GetAddress() > 0) { return true; }

        if (parentNode.Address.Length == 1 && node.Address.Length > 1)
        {
            return parentNode.Address[0] == node.Address[0];
        }

        if (parentNode.Address.Length == 2 && node.Address.Length > 2)
        {
            return parentNode.Address[0] == node.Address[0] && parentNode.Address[1] == node.Address[1];
        }

        if (parentNode.Address.Length == 3 && node.Address.Length > 3)
        {
            return parentNode.Address[0] == node.Address[0]
                   && parentNode.Address[1] == node.Address[1]
                   && parentNode.Address[2] == node.Address[2];
        }

        return false;
    }
}