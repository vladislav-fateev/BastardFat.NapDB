using BastardFat.NapDB.Abstractions;
using System.Collections.Generic;

namespace BastardFat.NapDB.Caching
{
    internal class MemoryCacheFactory
    {
        private static Dictionary<string, object> _caches = new Dictionary<string, object>();
        public MemoryCache<TKey, TEntity> GetMemoryCache<TKey, TEntity>(string name)
            where TEntity : class, IEntity<TKey>, new()
        {
            if (!_caches.ContainsKey(name) || !(_caches[name] is MemoryCache<TKey, TEntity> existing))
            {
                MemoryCache<TKey, TEntity> cache = new MemoryCache<TKey, TEntity>();
                _caches.Add(name, cache);
                return cache;
            }
            return existing;
        }
    }
}
