using BastardFat.NapDB.Abstractions;
using System;
using System.Linq.Expressions;

namespace BastardFat.NapDB.Config.Builders
{
    public interface IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {

        IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> HasOneReferenceTo<TRefEntity>(Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referenceTo)
            where TRefEntity : class, IEntity<TKey>, new();

        IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> HasManyReferencesTo<TRefEntity>(Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referencesTo)
            where TRefEntity : class, IEntity<TKey>, new();

        IBackRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> IsBackReferencesTo<TRefEntity>(Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referencesTo)
            where TRefEntity : class, IEntity<TKey>, new();

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> Index(string indexName);

        IDataSetConfigBuilder<TDb, TKey, TEntity> BuildPropertyConfiguration();

    }
}