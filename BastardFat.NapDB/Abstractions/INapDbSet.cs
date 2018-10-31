using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbSet
    {
        string Name { get; }
        string GetFolderName();
        int Count();
    }

    public interface INapDbSet<TKey>: INapDbSet
    {
        void Initialize(INapDb<TKey> database, string name);

        INapDb<TKey> Database { get; }
    }

    public interface INapDbSet<TEntity, TKey> : INapDbSet<TKey>, IEnumerable<TEntity> 
        where TEntity : class, INapDbEntity<TKey>, new()
    {
        TEntity Find(TKey id);
        IEnumerable<TEntity> Find(IEnumerable<TKey> ids);
        TEntity Save(TEntity entity);
        IEnumerable<TEntity> WherePropertyIs<TValue>(string propertyName, TValue value);
        void Delete(TKey id);
    }

    public interface INapDbSet<TEntity, TMeta, TKey> : INapDbSet<TEntity, TKey>
         where TEntity : class, INapDbEntity<TKey>, new()
         where TMeta : class, INapDbMeta<TKey>, new()
    {
        TMeta GetMetaObject();
        TMeta SaveMetaObject(TMeta meta);
    }

    

}
