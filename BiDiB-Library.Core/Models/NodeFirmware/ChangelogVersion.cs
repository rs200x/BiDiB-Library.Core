using System;
using System.Globalization;

namespace org.bidib.netbidibc.core.Models.NodeFirmware
{
    public class ChangelogVersion
    {
        public string Version { get; set; }

        public string ReleaseDate { get; set; }

        public DateTime ReleaseDateDate => !string.IsNullOrEmpty(ReleaseDate)
            ? DateTime.ParseExact(ReleaseDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None)
            : DateTime.MinValue;

        public ChangelogDescription[] Changelog { get; set; }

        public override string ToString() => $"{Version} - {ReleaseDateDate.ToShortDateString()}";
    }
}