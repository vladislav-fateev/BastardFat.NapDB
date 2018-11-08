using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Assurance;
using BastardFat.NapDB.Helpers;
using System;
using System.Linq.Expressions;

namespace BastardFat.NapDB.Config.Builders
{
    internal class EntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> : IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        private readonly EntityPropertyConfiguration<TDb, TKey> _propertyConfig;
        private readonly IDataSetConfigBuilder<TDb, TKey, TEntity> _parentBuilder;

        public EntityPropertyConfigBuilder(EntityPropertyConfiguration<TDb, TKey> propertyConfig, IDataSetConfigBuilder<TDb, TKey, TEntity> parentBuilder)
        {
            _propertyConfig = propertyConfig;
            _parentBuilder = parentBuilder;
        }

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>
            IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>.Index
            (string indexName)
        {
            _propertyConfig.IsIndexed = true;
            _propertyConfig.IndexName = indexName;
            return this;
        }

        IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> 
            IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>.HasManyReferencesTo<TRefEntity>
            (Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referencesTo)
        {
            InitReference(referencesTo);
            return new RelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>(_propertyConfig.Reference, this);
        }

        IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> 
            IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>.HasOneReferenceTo<TRefEntity>
            (Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referenceTo)
        {
            InitReference(referenceTo);
            return new RelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>(_propertyConfig.Reference, this);
        }

        IBackRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> 
            IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>.IsBackReferencesTo<TRefEntity>
            (Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referencesTo)
        {
            InitReference(referencesTo);
            return new RelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>(_propertyConfig.Reference, this);
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity>
            IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>.BuildPropertyConfiguration()
        {
            return _parentBuilder;
        }

        private void InitReference<TRefEntity>(Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referencesTo)
            where TRefEntity : class, IEntity<TKey>, new()
        {
            _propertyConfig.IsReference = true;

            referencesTo.GetPropertyInfo();
            var referencedSet = referencesTo.Compile().Invoke(_propertyConfig.DataSetConfig.DbConfig.Db);

            ConfigAssure.DataSetIsInitialized<TDb, TKey>(referencedSet);
            ConfigAssure.ConfigContainsDataSet(_propertyConfig.DataSetConfig.DbConfig, referencedSet);

            _propertyConfig.Reference = new ReferenceConfiguration<TKey> { SourceDataSet = referencedSet };
        }
    }
}
