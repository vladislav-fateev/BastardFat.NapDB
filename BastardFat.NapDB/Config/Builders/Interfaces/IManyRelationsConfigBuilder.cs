using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BastardFat.NapDB.Config.Builders
{
    public interface IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> : IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TRefEntity : class, IEntity<TKey>, new()
    {

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingForeignKeys(Expression<Func<TEntity, IEnumerable<TKey>>> foreignKeys);

    }
}