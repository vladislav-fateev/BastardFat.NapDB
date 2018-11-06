using BastardFat.NapDB.Locking;
using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface INapDb<TKey>
    {
        string GetRootDirectory();

        IDataSet<TKey> DataSet(string name);

        IDataSet<TEntity, TKey> DataSet<TEntity>() where TEntity : class, IEntity<TKey>, new();

        IDataSet<TEntity, TKey> DataSet<TEntity>(string name) where TEntity : class, IEntity<TKey>, new();

        IEnumerable<IDataSet<TKey>> AllDataSets();

        LockerWrapper BeginLock();
    }
}
