using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.Metadatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Tests.Support
{
    public class SmokeTestDb : NapDb<int, SmokeTestDb>
    {
        public SmokeTestDb() : base()
        {
        }

        protected override void Configure(INapDbConfigBuilder<SmokeTestDb, int> builder)
        {
            builder
                .UseRootFolderPath(SmokeTest.Path)
                .ConfigureDataSet(x => x.SmokeTestDataset)
                    .UseFolderName("_smoke")
                    .BuildDataSetConfiguration();
        }

        public IDataSet<SmokeTestEntity, Int32IncrementMetadata, int> SmokeTestDataset { get; set; }
    }

    public class SmokeTestEntity : IEntity<int>
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public DateTime Created { get; set; }
    }
}
