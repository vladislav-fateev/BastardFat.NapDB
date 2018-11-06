using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BastardFat.NapDB.Config.Builders
{
    public interface IBackRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> : IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TRefEntity : class, IEntity<TKey>, new()
    {

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingOneForeignKey(Expression<Func<TRefEntity, TKey>> foreignKey);

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingManyForeignKeys(Expression<Func<TRefEntity, IEnumerable<TKey>>> foreignKeys);

    }
}