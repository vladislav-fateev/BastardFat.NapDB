using System;
using System.Linq;
using System.Collections.Generic;
using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.Metadatas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BastardFat.NapDB.Tests
{
    public class SmokeTestDb : NapDb<int, SmokeTestDb>
    {
        protected override void Configure(INapDbConfigBuilder<SmokeTestDb, int> builder)
        {
            builder
                .UseRootFolderPath(SmokeTest.Path)
                .ConfigureDataSet(x => x.SmokeTestDataset)
                .UseFolderName("_smoke");
        }

        public IDataSet<SmokeTestEntity, Int32IncrementMetadata, int> SmokeTestDataset { get; set; }
    }

    public class SmokeTestEntity : IEntity<int>
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public DateTime Created { get; set; }
    }

    [TestClass]
    public class SmokeTest
    {
        public const string Path = @"C:\dev\BastardFat.NapDB\_databases\SmokeTestDb";

        [TestMethod]
        public void Simple()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path);

            var db = new SmokeTestDb();
            for (int i = 0; i < 5; i++)
            {
                db.SmokeTestDataset.Save(new SmokeTestEntity
                {
                    Created = DateTime.Now,
                    Data = "Test " + i,
                });
            }

            Assert.AreEqual(db.SmokeTestDataset.Count(), db.SmokeTestDataset.Count<SmokeTestEntity>());
            Assert.AreEqual(db.SmokeTestDataset.Count(), 5);

            var arr = db.SmokeTestDataset.ToArray();

            Assert.AreEqual(arr.Length, 5);
            Assert.AreEqual(arr[3].Data, "Test 3");

        }
    }
}
