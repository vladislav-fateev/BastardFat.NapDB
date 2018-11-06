using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.Helpers;
using BastardFat.NapDB.Locking;
using BastardFat.NapDB.Proxy;
using BastardFat.NapDB.Proxy.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BastardFat.NapDB
{
    public abstract class NapDb<TKey, TDatabase> : INapDb<TKey>
        where TDatabase : INapDb<TKey>
    {
        protected NapDb()
        {
            FillDataSets();
        }

        internal bool ConfigurationApplied { get; set; } = false;

        private string _rootDirectory;
        private ILocker _locker;
        public bool IsLocked => _locker.IsLocked;
        private static readonly object _configSyncRoot = new object();
        private static readonly object _lockingSyncRoot = new object();
        private readonly ProxyGeneratorFactory _proxyGeneratorFactory = new ProxyGeneratorFactory();
        private readonly List<IDataSet<TKey>> _dataSets = new List<IDataSet<TKey>>();
        

        private void FillDataSets()
        {
            var datasetProps = GetType()
                .GetProperties()
                .Where(x =>
                    x.CanRead &&
                    x.CanWrite &&
                    ReflectionHelper.IsPropertyGenericInterfaceDefinition(x, typeof(IDataSet<,,>)));

            foreach (var prop in datasetProps)
            {
                var genericArgs = prop.PropertyType.GetGenericArguments();
                if (genericArgs[2] != typeof(TKey))
                    continue;
                
                var datasetType = typeof(DataSet<,,>).MakeGenericType(genericArgs);
                
                var dataset = _proxyGeneratorFactory.GetProxyGenerator()
                    .CreateClassProxy(datasetType, new object[] { this, prop.Name }, new DataSetInterceptor(RunConfiguration));

                prop.SetValue(this, dataset);
                _dataSets.Add((IDataSet<TKey>)dataset);
            }
        }

        private void RunConfiguration()
        {
            if (ConfigurationApplied) return;

            lock (_configSyncRoot)
            {
                if (ConfigurationApplied) return;
                ConfigurationApplied = true;
                Type configType = typeof(NapDbConfiguration<,>)
                    .MakeGenericType(new[] { typeof(TDatabase), typeof(TKey) });

                var config = configType
                    .GetConstructor(new[] { typeof(TDatabase) })
                    .Invoke(new[] { this });

                var builder = typeof(NapDbConfigBuilder<,>)
                    .MakeGenericType(new[] { typeof(TDatabase), typeof(TKey) })
                    .GetConstructor(new[] { configType })
                    .Invoke(new[] { config });

                Configure(builder as INapDbConfigBuilder<TDatabase, TKey>);
                ApplyConfig(config as NapDbConfiguration<TDatabase, TKey>);
            }
        }

        private void ApplyConfig(NapDbConfiguration<TDatabase, TKey> config) 
        {
            _rootDirectory = config.RootPath;
            _locker = config.Locker;
            foreach (var datasetConfig in config.DataSetConfigs)
            {
                var ds = datasetConfig.Value.DataSet as DataSet<TKey>;
                ds.FolderName = datasetConfig.Value.FolderName;
                ds.EnableCaching = datasetConfig.Value.EnableCaching;
                ds.Serializer = datasetConfig.Value.Serializer;
                ds.NameResolver = datasetConfig.Value.NameResolver;
                ds.Reader = datasetConfig.Value.Reader;
            }
        }


        protected abstract void Configure(INapDbConfigBuilder<TDatabase, TKey> builder);

        public string GetRootDirectory()
        {
            RunConfiguration();
            return _rootDirectory;
        }

        public IDataSet<TKey> DataSet(string name)
        {
            RunConfiguration();
            return _dataSets.FirstOrDefault(x => x.Name == name);
        }

        public IDataSet<TEntity, TKey> DataSet<TEntity>()
            where TEntity : class, IEntity<TKey>, new()
        {
            RunConfiguration();
            return _dataSets.FirstOrDefault(x => x.GetEntityType() == typeof(TEntity)) as IDataSet<TEntity, TKey>;
        }

        public IDataSet<TEntity, TKey> DataSet<TEntity>(string name)
            where TEntity : class, IEntity<TKey>, new()
        {
            RunConfiguration();
            return _dataSets.FirstOrDefault(x => x.Name == name && x.GetEntityType() == typeof(TEntity)) as IDataSet<TEntity, TKey>;
        }

        public IEnumerable<IDataSet<TKey>> AllDataSets()
        {
            RunConfiguration();
            return _dataSets.ToArray();
        }

        public LockerWrapper BeginLock()
        {
            RunConfiguration();
            return new LockerWrapper(_locker, OnBeginLock, OnEndLock);
        }

        private void OnBeginLock()
        {
            foreach (var ds in _dataSets)
                (ds as DataSet<TKey>).MetaCacheLock();
        }
        private void OnEndLock()
        {
            foreach (var ds in _dataSets)
                (ds as DataSet<TKey>).MetaCacheUnlock();
        }
    }
}
