using System.Runtime.Serialization;

namespace org.bidib.netbidibc.core.Models.NodeFirmware
{
    [DataContract]
    public class ChangelogDescription
    {
        [DataMember(Name = "lang")]
        public string Lang { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}