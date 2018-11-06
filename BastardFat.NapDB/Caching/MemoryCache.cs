using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Abstractions.DataStructs;
using System;
using System.Collections.Generic;

namespace BastardFat.NapDB.Caching
{
    internal class MemoryCache<TKey, TEntity>
        where TEntity : class, IEntity<TKey>, new()
    {
        public TimeSpan Lifespan { get; }

        private readonly Dictionary<TKey, CachedValue<TEntity>> _cache = new Dictionary<TKey, CachedValue<TEntity>>();
        private readonly Dictionary<SearchQuery, CachedSearchResult> _searchCache = new Dictionary<SearchQuery, CachedSearchResult>();

        public MemoryCache() : this(TimeSpan.FromMinutes(5)) { }
        public MemoryCache(TimeSpan lifespan)
        {
            Lifespan = lifespan;
        }


        public virtual TEntity CacheValue(TEntity entity, string signature)
        {
            _cache[entity.Id] = new CachedValue<TEntity>
            {
                CachedAt = DateTime.Now,
                Value = entity,
                Signature = signature,
            };
            return entity;
        }
        public virtual bool IsCached(TKey id, string signature)
        {
            if (!_cache.ContainsKey(id))
                return false;
            if( _cache[id].Signature == signature &&  _cache[id].CachedAt.Add(Lifespan) > DateTime.Now)
                return true;
            else
            {
                _cache.Remove(id);
                return false;
            }
        }
        public virtual TEntity GetCachedValue(TKey id)
        {
            return _cache[id].Value;
        }
        public virtual void RemoveCached(TKey id)
        {
            if (_cache.ContainsKey(id))
                _cache.Remove(id);
        }


        public virtual bool GetCachedSearchResult(string folderPath, string pattern, out CachedSearchResult result)
        {
            var key = new SearchQuery(folderPath, pattern);
            if (_searchCache.ContainsKey(key))
            {
                result = _searchCache[key];
                return true;
            }
            result = new CachedSearchResult();
            return false;
        }
        public virtual void CacheSearchResult(string folderPath, string pattern, IEnumerable<string> result)
        {
            _searchCache[new SearchQuery(folderPath, pattern)] = new CachedSearchResult()
            {
                Result = result,
                CachedAt = DateTime.Now
            };
        }

    }
}
