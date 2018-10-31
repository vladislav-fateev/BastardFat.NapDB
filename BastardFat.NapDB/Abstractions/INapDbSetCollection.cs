using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbSetCollection<TKey>
    {
        void Add<TEntity>(IDataSet<TEntity, TKey> set)
            where TEntity : class, IEntity<TKey>, new();

        IDataSet<TEntity, TKey> GetSetByEntityType<TEntity>()
            where TEntity : class, IEntity<TKey>, new();

        IDataSet<TEntity, TKey> GetSetByEntityTypeAndName<TEntity>(string name)
            where TEntity : class, IEntity<TKey>, new();
    }
}
