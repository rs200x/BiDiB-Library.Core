using System;
using System.Timers;
using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models.BiDiB
{
    public sealed class OccupancyInfo : ModelBase, IDisposable
    {
        private ushort address;
        private int speed;
        private DecoderDirection direction;
        private int? quality;
        private int? temperature;
        private int? container1;
        private int? container2;
        private int? container3;
        private DateTime lastUpdate;
        private readonly Timer resetTimer;
        private bool isAlive;
        private double? trackVoltage;

        public OccupancyInfo()
        {
            resetTimer = new Timer(5000) { AutoReset = false };
            resetTimer.Elapsed += ResetTimer_Elapsed;
        }

        public ushort Address
        {
            get => address;
            set { Set(() => Address, ref address, value); }
        }

        public int Speed
        {
            get => speed;
            set { Set(() => Speed, ref speed, value); }
        }

        public DecoderDirection Direction
        {
            get => direction;
            set { Set(() => Direction, ref direction, value); }
        }

        public int? Quality
        {
            get => quality;
            set { Set(() => Quality, ref quality, value); }
        }

        public int? Temperature
        {
            get => temperature;
            set { Set(() => Temperature, ref temperature, value); }
        }

        public int? Container1
        {
            get => container1;
            set { Set(() => Container1, ref container1, value); }
        }

        public int? Container2
        {
            get => container2;
            set { Set(() => Container2, ref container2, value); }
        }

        public int? Container3
        {
            get => container3;
            set { Set(() => Container3, ref container3, value); }
        }

        public double? TrackVoltage
        {
            get => trackVoltage;
            set => Set(() => TrackVoltage, ref trackVoltage, value);
        }

        public DateTime LastUpdate
        {
            get => lastUpdate;
            set => Set(() => LastUpdate, ref lastUpdate, value);
        }

        public bool IsAlive
        {
            get => isAlive;
            set => Set(()=>IsAlive, ref isAlive, value);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if(propertyName == nameof(LastUpdate) || propertyName == nameof(IsAlive)) { return; }

            LastUpdate = DateTime.Now;
            IsAlive = true;
            resetTimer.Start();
        }

        private void ResetTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            resetTimer.Stop();
            IsAlive = false;
        }
        
        public void Dispose()
        {
            resetTimer.Stop();
            resetTimer.Elapsed -= ResetTimer_Elapsed;
            resetTimer.Dispose();
        }

        public override string ToString() => $"OccupancyInfo A:{address}";
    }
}