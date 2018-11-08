using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.Metadatas;

namespace BastardFat.NapDB.Tests.Smoke
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
                    .EnableCaching()
                    .UseFolderName("_smoke")
                    .BuildDataSetConfiguration();
        }

        public IDataSet<SmokeTestEntity, Int32IncrementMetadata, int> SmokeTestDataset { get; set; }
    }
}
