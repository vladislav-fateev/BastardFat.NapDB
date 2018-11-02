using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.Exceptions;
using BastardFat.NapDB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB
{
    public abstract class NapDb<TKey, TDatabase> : INapDb<TKey>
        where TDatabase : INapDb<TKey>
    {
        protected NapDb()
        {
            FillDataSets();
            RunConfiguration();
        }

        private string _rootDirectory;

        private void FillDataSets()
        {
            var datasetProps = GetType()
                .GetProperties()
                .Where(
                    x => x.CanRead &&
                    x.CanWrite &&
                    x.PropertyType.IsGenericType &&
                    typeof(IDataSet<,,>).IsAssignableFrom(x.PropertyType.GetGenericTypeDefinition()));
            foreach (var prop in datasetProps)
            {
                var iface = prop.PropertyType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataSet<,,>));
                if (prop.PropertyType.IsInterface && prop.PropertyType.GetGenericTypeDefinition() == typeof(IDataSet<,,>))
                    iface = prop.PropertyType;
                if (iface == null)
                    continue;
                var genericArgs = iface.GetGenericArguments();
                var tEntity = genericArgs[0];
                var tMeta = genericArgs[1];
                var tKey = genericArgs[2];
                if (tKey != typeof(TKey))
                    continue;
                var datasetType = typeof(DataSet<,,>).MakeGenericType(genericArgs);
                var constructor = datasetType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(INapDb<TKey>), typeof(string) }, null);
                var dataset = constructor.Invoke(new object[] { this, prop.Name });
                prop.SetValue(this, dataset);
                _dataSets.Add((IDataSet<TKey>)dataset);
            }
        }

        private void RunConfiguration()
        {
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

        private void ApplyConfig(NapDbConfiguration<TDatabase, TKey> config) 
        {
            _rootDirectory = config.RootPath;
            foreach (var datasetConfig in config.DataSetConfigs)
            {
                var ds = datasetConfig.Value.DataSet as DataSet<TKey>;
                ds.FolderName = datasetConfig.Value.FolderName;
                ds.IsReadOnly = datasetConfig.Value.IsReadOnly;
                ds.Serializer = datasetConfig.Value.Serializer;
                ds.NameResolver = datasetConfig.Value.NameResolver;
                ds.Reader = datasetConfig.Value.Reader;
            }
        }

        private readonly List<IDataSet<TKey>> _dataSets = new List<IDataSet<TKey>>();

        protected abstract void Configure(INapDbConfigBuilder<TDatabase, TKey> builder);

        public string GetRootDirectory()
        {
            return _rootDirectory;
        }

        public IDataSet<TKey> DataSet(string name)
        {
            return _dataSets.FirstOrDefault(x => x.Name == name);
        }

        public IDataSet<TEntity, TKey> DataSet<TEntity>()
            where TEntity : class, IEntity<TKey>, new()
        {
            return _dataSets.FirstOrDefault(x => x.GetEntityType() == typeof(TEntity)) as IDataSet<TEntity, TKey>;
        }

        public IDataSet<TEntity, TKey> DataSet<TEntity>(string name)
            where TEntity : class, IEntity<TKey>, new()
        {
            return _dataSets.FirstOrDefault(x => x.Name == name && x.GetEntityType() == typeof(TEntity)) as IDataSet<TEntity, TKey>;
        }

        public IEnumerable<IDataSet<TKey>> AllDataSets()
        {
            return _dataSets.ToArray();
        }
    }

    /// <summary>
    /// JUST DRAFT!!! Will be removed
    /// </summary>
    public class MutexWrapper : IDisposable
    {
        private System.Threading.Mutex mutex = new System.Threading.Mutex(false, "testmutex");
        private MutexWrapper()
        {
            mutex.WaitOne();
        }
        public void Dispose()
        {
            mutex.ReleaseMutex();
        }

        public static MutexWrapper Lock()
        {
            return new MutexWrapper();
        }
    }
}
