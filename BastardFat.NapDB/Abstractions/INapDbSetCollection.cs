using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbSetCollection<TKey>
    {
        void Add<TEntity>(INapDbSet<TEntity, TKey> set)
            where TEntity : class, INapDbEntity<TKey>, new();

        INapDbSet<TEntity, TKey> GetSetByEntityType<TEntity>()
            where TEntity : class, INapDbEntity<TKey>, new();

        INapDbSet<TEntity, TKey> GetSetByEntityTypeAndName<TEntity>(string name)
            where TEntity : class, INapDbEntity<TKey>, new();
    }
}
