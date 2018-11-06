using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BastardFat.NapDB.Config
{
    internal class NapDbConfiguration<TDb, TKey>
        where TDb : INapDb<TKey>
    {
        public NapDbConfiguration(TDb db)
        {
            Db = db;

            RootPath = Path.Combine(Environment.CurrentDirectory, "Data", typeof(TDb).Name);
            Locker = GlobalDefaults.CreateDefaultLocker(db.GetType().FullName);

            DataSetConfigs = db
                .AllDataSets()
                .ToDictionary(
                    ds => ds.Name,
                    ds => new DataSetConfiguration<TDb, TKey>(this, ds));
        }

        public TDb Db { get; }

        public string RootPath { get; set; }
        public ILocker Locker { get; set; }

        public Dictionary<string, DataSetConfiguration<TDb, TKey>> DataSetConfigs { get; }
    }
}
