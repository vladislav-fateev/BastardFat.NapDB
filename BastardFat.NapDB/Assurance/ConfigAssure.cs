using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config;
using BastardFat.NapDB.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Assurance
{
    internal static class ConfigAssure
    {

        public static void DataSetIsInitialized<TDb, TKey>(IDataSet<TKey> dataset)
            where TDb : INapDb<TKey>
        {
            if (dataset == null)
                throw new NapDbException(typeof(TDb).Name, $"Dataset '{dataset.Name}' has not been initialized");
        }

        public static void ConfigContainsDataSet<TDb, TKey>(NapDbConfiguration<TDb, TKey> config, IDataSet<TKey> dataset)
            where TDb : INapDb<TKey>
        {
            if (!config.DataSetConfigs.ContainsKey(dataset.Name))
                throw new NapDbException(typeof(TDb).Name, $"Configuration doesn't contains dataset '{dataset.Name}'");

            if (config.DataSetConfigs[dataset.Name].DataSet != dataset)
                throw new NapDbException(typeof(TDb).Name, $"Configuration of dataset '{dataset.Name}' is corrupted");
        }

        public static void EntityConfigContainsConfigurableProperty<TDb, TKey>(DataSetConfiguration<TDb, TKey> config, PropertyInfo prop)
             where TDb : INapDb<TKey>
        {
            if (!config.PropertyConfigs.ContainsKey(prop.Name))
                throw new NapDbException(typeof(TDb).Name, $"Dataset '{config.DataSet.Name}' doesn't contains property '{prop.Name}' or it's not configurable");
        }

        
    }
}
