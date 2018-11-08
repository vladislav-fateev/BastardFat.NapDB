using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.FileSystem;
using BastardFat.NapDB.Metadatas;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Tests.Performance
{
    public class PerformanceTestDb : NapDb<int, PerformanceTestDb>
    {
        private readonly bool _cachng;
        public PerformanceTestDb(bool cachng) : base()
        {
            _cachng = cachng;
            PerformanceFileReader = new PerformanceFileReader(FileReaderFolderCreationMode.CreateWhenWrite | FileReaderFolderCreationMode.CreateWhenRead | FileReaderFolderCreationMode.CreateWhenSearch);
            PerformanceXmlSerializer = new PerformanceXmlSerializer<int>();
        }

        public PerformanceFileReader PerformanceFileReader { get; }
        public PerformanceXmlSerializer<int> PerformanceXmlSerializer { get; }

        protected override void Configure(INapDbConfigBuilder<PerformanceTestDb, int> builder)
        {
            var dscb = builder
                .UseRootFolderPath(PerformanceTest.Path + "\\")
                .ConfigureDataSet(x => x.PerformanceTestDataset);

            if (_cachng)
                dscb.EnableCaching();
            else
                dscb.DisableCaching();

            dscb.UseFolderName(PerformanceTest.EntityFolder)
                .UseCustomReader(PerformanceFileReader)
                .UseCustomSerializer(PerformanceXmlSerializer)
                .BuildDataSetConfiguration();
        }

        public IDataSet<PerformanceTestEntity, Int32IncrementMetadata, int> PerformanceTestDataset { get; set; }
    }
}
