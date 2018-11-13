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
using System.Threading.Tasks;
using System.Threading;

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

        [TestMethod]
        public void ConcurrencyTest()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);

            const int count = 100;

            var db = new SmokeTestDb();
            var first = db.SmokeTestDataset.Save(new SmokeTestEntity
            {
                Created = DateTime.Today,
                Data = "Iterations: " + Environment.NewLine,
                Counter = 0
            });
            var firstId = first.Id;

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < count; i++)
            {
                tasks.Add(new Task(state =>
                {
                    var iter = (int)state;
                    var dbInner = new SmokeTestDb();
                    using (dbInner.BeginLock())
                    {
                        var f = dbInner.SmokeTestDataset.Find(firstId);
                        var counter = f.Counter;
                        var data = f.Data;
                        Thread.Sleep(100);
                        f.Counter = counter + 1;
                        f.Data = data + iter.ToString() + Environment.NewLine;
                        dbInner.SmokeTestDataset.Save(f);
                    }
                }, i));
            }

            Parallel.ForEach(tasks, t => t.Start());

            Task.WhenAll(tasks).Wait();

            Trace.TraceInformation(db.SmokeTestDataset.Find(firstId).Data);

            Assert.AreEqual(count, db.SmokeTestDataset.Find(firstId).Counter);
        }
    }
}
