using System;
using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Models.Decoder;

public readonly struct DecoderInfo : IEquatable<DecoderInfo>
{
    public DecoderInfo(ushort address, DecoderDirection direction)
    {
        Address = address;
        Direction = direction;
    }

    public ushort Address { get; }

    public DecoderDirection Direction { get; }

    public override string ToString() => $"{Address}/{Direction}";

    public bool Equals(DecoderInfo other)
    {
        return Address == other.Address && Direction == other.Direction;
    }

    public override bool Equals(object obj)
    {
        return obj is DecoderInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Address.GetHashCode() * 397) ^ (int)Direction;
        }
    }
}