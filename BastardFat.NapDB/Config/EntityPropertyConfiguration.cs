using BastardFat.NapDB.Abstractions;
using System.Reflection;

namespace BastardFat.NapDB.Config
{
    internal class EntityPropertyConfiguration<TDb, TKey>
        where TDb : INapDb<TKey>
    {
        public EntityPropertyConfiguration(DataSetConfiguration<TDb, TKey> dataSetConfig, PropertyInfo prop)
        {
            DataSetConfig = dataSetConfig;
            Property = prop;

            IsIndexed = false;
            IsReference = false;
            IndexName = null;
            Reference = null;
        }

        public DataSetConfiguration<TDb, TKey> DataSetConfig { get; }
        public PropertyInfo Property { get; }

        public bool IsIndexed { get; set; }
        public string IndexName { get; set; }
        public bool IsReference { get; set; }
        public ReferenceConfiguration<TDb, TKey> Reference { get; set; }
    }
}
