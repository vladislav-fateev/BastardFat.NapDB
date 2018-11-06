using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace BastardFat.NapDB.Config
{
    internal class DataSetConfiguration<TDb, TKey>
        where TDb : INapDb<TKey>
    {
        public DataSetConfiguration(NapDbConfiguration<TDb, TKey> dbConfig, IDataSet<TKey> dataSet)
        {
            DbConfig = dbConfig;
            DataSet = dataSet;

            FolderName = dataSet.Name;
            EnableCaching = false;
            Serializer = GlobalDefaults.CreateDefaultSerializer<TKey>();
            Reader = GlobalDefaults.CreateDefaultReader();
            NameResolver = GlobalDefaults.CreateDefaultNameResolver<TKey>();

            PropertyConfigs = dataSet
                .GetEntityType()
                .GetProperties()
                .Where(ReflectionHelper.IsPropertyConfigurable)
                .ToDictionary(
                    p => p.Name, 
                    p => new EntityPropertyConfiguration<TDb, TKey>(this, p));
        }

        public NapDbConfiguration<TDb, TKey> DbConfig { get; }
        public IDataSet<TKey> DataSet { get; }

        public string FolderName { get; set; }
        public bool EnableCaching { get; set; }
        public IEntitySerializer<TKey> Serializer { get; set; }
        public IFileReader Reader { get; set; }
        public IFileNameResolver<TKey> NameResolver { get; set; }

        public Dictionary<string, EntityPropertyConfiguration<TDb, TKey>> PropertyConfigs { get; set; }
    }
}
