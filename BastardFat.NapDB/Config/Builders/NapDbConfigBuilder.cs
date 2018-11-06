using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Assurance;
using BastardFat.NapDB.Helpers;
using System;
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

        INapDbConfigBuilder<TDb, TKey> INapDbConfigBuilder<TDb, TKey>.UseCustomLocker(ILocker locker)
        {
            _config.Locker = locker;
            return this;
        }
    }
}
