using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BastardFat.NapDB.Config.Builders
{
    internal class RelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> :
        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>,
        IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>,
        IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>,
        IBackRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TRefEntity : class, IEntity<TKey>, new()
    {
        private readonly ReferenceConfiguration<TDb, TKey> _refConfig;
        private readonly IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> _parentBuilder;

        public RelationConfigBuilder(
            ReferenceConfiguration<TDb, TKey> refConfig,
            IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> parentBuilder)
        {
            _refConfig = refConfig;
            _parentBuilder = parentBuilder;
        }

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> 
            IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>.UsingForeignKey
            (Expression<Func<TEntity, TKey>> foreignKey)
        {
            var foreignKeyProp = foreignKey.GetPropertyInfo();
            _refConfig.ForeignKeyProperty = foreignKeyProp;
            _refConfig.Kind = ReferenceKind.OneToMany;
            return this;
        }

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> 
            IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>.UsingForeignKeys
            (Expression<Func<TEntity, IEnumerable<TKey>>> foreignKeys)
        {
            var foreignKeyProp = foreignKeys.GetPropertyInfo();
            _refConfig.ForeignKeyProperty = foreignKeyProp;
            _refConfig.Kind = ReferenceKind.ManyToMany;
            return this;
        }

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> 
            IBackRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>.UsingManyForeignKeys
            (Expression<Func<TRefEntity, IEnumerable<TKey>>> foreignKeys)
        {
            var foreignKeyProp = foreignKeys.GetPropertyInfo();
            _refConfig.ForeignKeyProperty = foreignKeyProp;
            _refConfig.Kind = ReferenceKind.BackFromMany;
            return this;
        }

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> 
            IBackRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>.UsingOneForeignKey
            (Expression<Func<TRefEntity, TKey>> foreignKey)
        {
            var foreignKeyProp = foreignKey.GetPropertyInfo();
            _refConfig.ForeignKeyProperty = foreignKeyProp;
            _refConfig.Kind = ReferenceKind.BackFromOne;
            return this;
        }

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> 
            IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>.BuildRelationConfiguration()
        {
            return _parentBuilder;
        }
    }
}
