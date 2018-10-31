using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbFileIO<TEntity, TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
    {
        IEnumerable<TKey> GetAllIds();

        TEntity Read(TKey id);

        TEntity Write(TEntity entity);

        void Remove(TKey id);
    }
}
