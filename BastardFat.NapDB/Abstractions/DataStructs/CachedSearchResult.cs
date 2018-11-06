using System;
using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions.DataStructs
{
    public struct CachedSearchResult
    {
        public IEnumerable<string> Result { get; set; }
        public DateTime CachedAt { get; set; }
    }
}
