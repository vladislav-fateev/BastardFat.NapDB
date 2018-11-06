using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Assurance;
using BastardFat.NapDB.Helpers;
using System;
using System.Linq.Expressions;

namespace BastardFat.NapDB.Config.Builders
{
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
            IDataSetConfigBuilder<TDb, TKey, TEntity>.DisableCaching()
        {
            _dataSetConfig.EnableCaching = false;
            return this;
        }

        IDataSetConfigBuilder<TDb, TKey, TEntity> 
            IDataSetConfigBuilder<TDb, TKey, TEntity>.EnableCaching()
        {
            _dataSetConfig.EnableCaching = true;
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
}
