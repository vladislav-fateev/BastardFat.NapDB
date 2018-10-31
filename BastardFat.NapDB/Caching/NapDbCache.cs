using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Caching
{
    public class NapDbCache<TEntity, TKey> : INapDbCache<TEntity, TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
    {
        private readonly Dictionary<TKey, NapDbCachedEntity<TEntity, TKey>> _cache;
        private readonly INapDbProxier _proxier;

        public NapDbCache(INapDbProxier proxier)
        {
            _cache = new Dictionary<TKey, NapDbCachedEntity<TEntity, TKey>>();
            _proxier = proxier;
        }

        public string GetCachedSignature(TKey id)
        {
            if (!_cache.ContainsKey(id))
                return null;
            return _cache[id].Signature;
        }

        public TEntity GetCachedValue(TKey id)
        {
            if (!_cache.ContainsKey(id))
                return null;
            return _cache[id].Entity;
        }

        public void RemoveCached(TKey id)
        {
            if (!_cache.ContainsKey(id))
                _cache.Remove(id);
        }

        public TEntity SaveToCache(TEntity entity, string signature)
        {
            if (!_proxier.IsProxiedObject(entity))
                entity = _proxier.Proxy(entity);

            _cache[entity.Id] = new NapDbCachedEntity<TEntity, TKey>
            {
                Entity = entity,
                Signature = signature,
                CachedAt = DateTime.Now
            };
            return entity;
        }
    }
}
