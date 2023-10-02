using System;
using System.Timers;

namespace org.bidib.Net.Core.Models.BiDiB;

public sealed class PositionPort : FeedbackPort, IDisposable
{
    private readonly Timer resetTimer;

    public PositionPort(ushort position)
    {
        Position = position;
        Number = position;

        resetTimer = new Timer(3000) { AutoReset = false };
        resetTimer.Elapsed += ResetTimer_Elapsed;
    }

    private void ResetTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        resetTimer.Stop();
        IsFree = true;
    }

    public ushort Position { get; }

    public override void AddOccupancy(OccupancyInfo occupancy)
    {
        Occupancies = new[] { occupancy };
        IsFree = false;
        resetTimer.Start();
        RaisePropertyChanged(() => IsOccupied);
    }

    public void Dispose()
    {
        resetTimer?.Dispose();
    }
}