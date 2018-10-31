using System;

namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbCache<TEntity, TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
    {
        TEntity GetCachedValue(TKey id);
        string GetCachedSignature(TKey id);

        TEntity SaveToCache(TEntity entity, string signature);
        void RemoveCached(TKey id);
    }

}
