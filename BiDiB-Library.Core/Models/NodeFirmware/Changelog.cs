namespace org.bidib.netbidibc.core.Models.NodeFirmware
{
    public class Changelog
    {
        public string Product { get; set; }

        public string Owner { get;set; }

        public ChangelogVersion[] Versions { get; set; }
    }
}