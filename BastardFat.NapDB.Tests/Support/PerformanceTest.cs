using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.FileSystem;
using BastardFat.NapDB.Metadatas;
using BastardFat.NapDB.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Tests.Support
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

    public class PerformanceTestEntity : IEntity<int>
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public DateTime Created { get; set; }
    }

    public class PerformanceFileReader : FileReader
    {
        public PerformanceFileReader(FileReaderFolderCreationMode creationMode) : base(creationMode)
        {
            Measurer = new Measurer();
        }
        public Measurer Measurer { get; }

        public override byte[] Read(string folderPath, string name)
        {
            return Measurer.Measure(() => base.Read(folderPath, name));
        }

        public override void Remove(string folderPath, string name)
        {
            Measurer.Measure(() => { base.Remove(folderPath, name); return true; });
        }

        public override IEnumerable<string> Search(string folderPath, string pattern)
        {
            return Measurer.Measure(() => base.Search(folderPath, pattern));
        }

        public override void Write(string folderPath, string name, byte[] content)
        {
            Measurer.Measure(() => { base.Write(folderPath, name, content); return true; });
        }

        public override bool HasChangedSince(string folderPath, DateTime since)
        {
            return Measurer.Measure(() => base.HasChangedSince(folderPath, since));
        }
    }




    public class Measurer
    {
        public Dictionary<string, Measurement> Measures = new Dictionary<string, Measurement>();

        public void Clear() { Measures.Clear(); }

        public T Measure<T>(Func<T> action, [CallerMemberName] string caller = "")
        {
            if (!Measures.ContainsKey(caller))
                Measures[caller] = new Measurement();
            Measures[caller].Calls++;
            Measures[caller].Time.Start();
            var result = action();
            Measures[caller].Time.Stop();
            return result;
        }

        public class Measurement
        {
            public Stopwatch Time { get; set; } = new Stopwatch();
            public int Calls { get; set; } = 0;
        }
    }

    public class PerformanceXmlSerializer<TKey> : DefaultXmlSerializer<TKey>
    {
        public PerformanceXmlSerializer() : base()
        {
            Measurer = new Measurer();
        }
        public Measurer Measurer { get; }
        public override TEntity Deserialize<TEntity>(byte[] content)
        {
            return Measurer.Measure(() => base.Deserialize<TEntity>(content));
        }

        public override string GetSignature(byte[] content)
        {
            return Measurer.Measure(() => base.GetSignature(content));
        }

        public override byte[] Serialize<TEntity>(TEntity model)
        {
            return Measurer.Measure(() => base.Serialize(model));
        }
    }
}
