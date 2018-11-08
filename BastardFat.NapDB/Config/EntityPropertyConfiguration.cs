using BastardFat.NapDB.Abstractions;
using System.Reflection;

namespace BastardFat.NapDB.Config
{
    internal class EntityPropertyConfiguration<TKey>
    {
        public PropertyInfo Property { get; protected set; }

        public bool IsIndexed { get; set; }
        public string IndexName { get; set; }
        public bool IsReference { get; set; }
        public ReferenceConfiguration<TKey> Reference { get; set; }
    }

    internal class EntityPropertyConfiguration<TDb, TKey> : EntityPropertyConfiguration<TKey>
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
    }
}
