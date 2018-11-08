using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.Metadatas;

namespace BastardFat.NapDB.Tests.Relations.OneToMany
{
    public class OneToManyTestDb : NapDb<int, OneToManyTestDb>
    {
        public OneToManyTestDb() : base()
        {
        }

        protected override void Configure(INapDbConfigBuilder<OneToManyTestDb, int> builder)
        {
            builder
                .UseRootFolderPath(OneToManyTest.Path)

                .ConfigureDataSet(x => x.Companies)
                    .EnableCaching()
                    .ConfigureProperty(x => x.Phone)
                        .HasOneReferenceTo(x => x.Phones)
                            .UsingForeignKey(x => x.PhoneId)
                            .BuildRelationConfiguration()
                        .BuildPropertyConfiguration()
                    .BuildDataSetConfiguration()

                .ConfigureDataSet(x => x.Phones)
                    .EnableCaching()
                    .BuildDataSetConfiguration();
        }

        public IDataSet<OneToManyTest_Company, Int32IncrementMetadata, int> Companies { get; set; }
        public IDataSet<OneToManyTest_Phone, Int32IncrementMetadata, int> Phones { get; set; }
    }
}
