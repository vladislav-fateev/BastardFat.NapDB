using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions.DataStructs
{
    internal struct SearchQuery
    {
        public SearchQuery(string folderPath, string pattern)
        {
            FolderPath = folderPath;
            Pattern = pattern;
        }

        public string FolderPath { get; set; }
        public string Pattern { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is SearchQuery query))
                return false;

            return FolderPath == query.FolderPath &&
                   Pattern == query.Pattern;
        }

        public override int GetHashCode()
        {
            var hashCode = 505783575;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FolderPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Pattern);
            return hashCode;
        }
    }
}
