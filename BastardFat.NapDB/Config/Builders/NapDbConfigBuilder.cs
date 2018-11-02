using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Assurance;
using BastardFat.NapDB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Config.Builders
{
    internal class NapDbConfigBuilder<TDb, TKey> : INapDbConfigBuilder<TDb, TKey>
        where TDb : INapDb<TKey>
    {
        private readonly NapDbConfiguration<TDb, TKey> _config;
        public NapDbConfigBuilder(NapDbConfiguration<TDb, TKey> config)
        {
            _config = config;
        }


        public INapDbConfigBuilder<TDb, TKey> Create(NapDbConfiguration<TDb, TKey> config)
        {
            return new NapDbConfigBuilder<TDb, TKey>(config);
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity> INapDbConfigBuilder<TDb, TKey>.ConfigureDataSet<TEntity>(Expression<Func<TDb, IDataSet<TEntity, TKey>>> set)
        {
            set.GetPropertyInfo();
            var dataSet = set.Compile().Invoke(_config.Db);

            ConfigAssure.DataSetIsInitialized<TDb, TKey>(dataSet);
            ConfigAssure.ConfigContainsDataSet(_config, dataSet);

            return new DataSetConfigBuilder<TDb, TKey, TEntity>(_config.DataSetConfigs[dataSet.Name], this);
        }

        INapDbConfigBuilder<TDb, TKey> INapDbConfigBuilder<TDb, TKey>.UseRootFolderPath(string path)
        {
            _config.RootPath = path;
            return this;
        }
    }

    internal class DataSetConfigBuilder<TDb, TKey, TEntity> : IDataSetConfigBuilder<TDb, TKey, TEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        private readonly DataSetConfiguration<TDb, TKey> _dataSetConfig;
        private readonly INapDbConfigBuilder<TDb, TKey> _parentBuilder;

        public DataSetConfigBuilder(
            DataSetConfiguration<TDb, TKey> dataSetConfig,
            INapDbConfigBuilder<TDb, TKey> parentBuilder)
        {
            _dataSetConfig = dataSetConfig;
            _parentBuilder = parentBuilder;
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity> 
            IDataSetConfigBuilder<TDb, TKey, TEntity>.UseFolderName
            (string name)
        {
            _dataSetConfig.FolderName = name;
            return this;
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity> 
            IDataSetConfigBuilder<TDb, TKey, TEntity>.AllowWrite()
        {
            _dataSetConfig.IsReadOnly = false;
            return this;
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity> 
            IDataSetConfigBuilder<TDb, TKey, TEntity>.MakeReadonly()
        {
            _dataSetConfig.IsReadOnly = true;
            return this;
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity> 
            IDataSetConfigBuilder<TDb, TKey, TEntity>.UseCustomReader
            (IFileReader reader)
        {
            _dataSetConfig.Reader = reader;
            return this;
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity>
            IDataSetConfigBuilder<TDb, TKey, TEntity>.UseCustomNameResolver
            (IFileNameResolver<TKey> resolver)
        {
            _dataSetConfig.NameResolver = resolver;
            return this;
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity> 
            IDataSetConfigBuilder<TDb, TKey, TEntity>.UseCustomSerializer
            (IEntitySerializer<TKey> serializer)
        {
            _dataSetConfig.Serializer = serializer;
            return this;
        }

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> 
            IDataSetConfigBuilder<TDb, TKey, TEntity>.ConfigureProperty<TProp>
            (Expression<Func<TEntity, TProp>> property)
        {
            var prop = property.GetPropertyInfo();

            ConfigAssure.EntityConfigContainsConfigurableProperty(_dataSetConfig, prop);

            return new EntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>(_dataSetConfig.PropertyConfigs[prop.Name], this);
        }

        INapDbConfigBuilder<TDb, TKey>
            IDataSetConfigBuilder<TDb, TKey, TEntity>.BuildDataSetConfiguration()
        {
            return _parentBuilder;
        }
    }

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

            _propertyConfig.Reference = new ReferenceConfiguration<TDb, TKey> { SourceDataSet = referencedSet };
        }
    }

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
