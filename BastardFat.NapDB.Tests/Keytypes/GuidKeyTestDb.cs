using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.Metadatas;

namespace BastardFat.NapDB.Tests.Keytypes
{
    public class GuidKeyTestDb : NapDb<string, GuidKeyTestDb>
    {
        public GuidKeyTestDb() : base() { }

        protected override void Configure(INapDbConfigBuilder<GuidKeyTestDb, string> builder)
        {
            builder
                .UseRootFolderPath(GuidKeyTest.Path)
                .ConfigureDataSet(x => x.GuidKeyTestDataset)
                    .DisableCaching()
                    .UseFolderName("_guid")
                    .BuildDataSetConfiguration();
        }

        public IDataSet<GuidKeyTestEntity, GuidMetadata, string> GuidKeyTestDataset { get; set; }
    }
}
