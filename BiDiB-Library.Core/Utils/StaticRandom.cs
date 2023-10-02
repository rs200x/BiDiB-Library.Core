using System;
using System.Threading;

namespace org.bidib.Net.Core.Utils;

public static class StaticRandom
{
    private static int seed = Environment.TickCount;

    private static readonly ThreadLocal<Random> ThreadLocal = new(() => new Random(Interlocked.Increment(ref seed)));


    public static Random Instance => ThreadLocal.Value;
}