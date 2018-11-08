using System;
using System.Linq;
using System.Collections.Generic;
using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config.Builders;
using BastardFat.NapDB.Metadatas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using BastardFat.NapDB.FileSystem;
using System.Diagnostics;

namespace BastardFat.NapDB.Tests.Smoke
{


    [TestClass]
    public class SmokeTest
    {
        public const string Path = @"C:\dev\BastardFat.NapDB\_databases\SmokeTestDb\";

        [TestMethod]
        public void Simple()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);

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
            Assert.AreEqual(arr[3].Data, db.SmokeTestDataset.Find(arr[3].Id).Data);
        }
    }
}
