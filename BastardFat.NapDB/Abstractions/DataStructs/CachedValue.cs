using System;

namespace BastardFat.NapDB.Abstractions.DataStructs
{
    internal struct CachedValue<T>
    {
        public T Value { get; set; }
        public string Signature { get; set; }
        public DateTime CachedAt { get; set; }
    }
}
