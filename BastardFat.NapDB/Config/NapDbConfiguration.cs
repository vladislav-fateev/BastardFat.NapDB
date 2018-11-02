using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BastardFat.NapDB.Config
{
    internal class NapDbConfiguration<TDb, TKey>
        where TDb : INapDb<TKey>
    {
        public NapDbConfiguration(TDb db)
        {
            Db = db;

            RootPath = Path.Combine(Environment.CurrentDirectory, "Data", typeof(TDb).Name);

            DataSetConfigs = db
                .AllDataSets()
                .ToDictionary(
                    ds => ds.Name,
                    ds => new DataSetConfiguration<TDb, TKey>(this, ds));
        }

        public TDb Db { get; }

        public string RootPath { get; set; }

        public Dictionary<string, DataSetConfiguration<TDb, TKey>> DataSetConfigs { get; }
    }

    internal class DataSetConfiguration<TDb, TKey>
        where TDb : INapDb<TKey>
    {
        public DataSetConfiguration(NapDbConfiguration<TDb, TKey> dbConfig, IDataSet<TKey> dataSet)
        {
            DbConfig = dbConfig;
            DataSet = dataSet;

            FolderName = dataSet.Name;
            IsReadOnly = false;
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
        public bool IsReadOnly { get; set; }
        public IEntitySerializer<TKey> Serializer { get; set; }
        public IFileReader Reader { get; set; }
        public IFileNameResolver<TKey> NameResolver { get; set; }

        public Dictionary<string, EntityPropertyConfiguration<TDb, TKey>> PropertyConfigs { get; set; }
    }

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

    internal class ReferenceConfiguration<TDb, TKey>
        where TDb : INapDb<TKey>
    {
        public ReferenceKind Kind { get; set; }
        public IDataSet<TKey> SourceDataSet { get; set; }
        public PropertyInfo ForeignKeyProperty { get; set; }
    }

    internal enum ReferenceKind
    {
        OneToMany,
        ManyToMany,
        BackFromOne,
        BackFromMany
    }
}
