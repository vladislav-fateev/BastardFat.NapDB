using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions.DataStructs
{
    public class FileNameSegments
    {
        public string Key { get; set; }
        public string Signature { get; set; }
        public Dictionary<string, string> AdditionalSegments { get; set; } = new Dictionary<string, string>();
    }
}
