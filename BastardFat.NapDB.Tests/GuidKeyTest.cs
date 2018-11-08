using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BastardFat.NapDB.Metadatas;
using BastardFat.NapDB.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BastardFat.NapDB.Tests
{
    [TestClass]
    public class GuidKeyTest
    {
        public const string Path = @"C:\dev\BastardFat.NapDB\_databases\GuidKeyTestDb\";

        [TestMethod]
        public void Simple()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);

            var first = new GuidKeyTestEntity { Data = "First" };
            var second = new GuidKeyTestEntity { Data = "Second" };

            var db = new GuidKeyTestDb();
            using (db.BeginLock())
            {
                db.GuidKeyTestDataset.Save(first);
                db.GuidKeyTestDataset.Save(second);
            }

            db = new GuidKeyTestDb(); // renew db

            var ids = db.GuidKeyTestDataset.Select(x => x.Id).ToArray();
            Assert.AreEqual(2, ids.Length);

            Assert.AreNotEqual(first, db.GuidKeyTestDataset.First(x => x.Data == "First"));
            Assert.AreEqual(first.Id, db.GuidKeyTestDataset.First(x => x.Data == "First").Id);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(first.Id) && first.Id != GuidMetadata.GetDefaultId() && first.Id != second.Id);


            Trace.TraceInformation("Default key: {0}", GuidMetadata.GetDefaultId());
            Trace.TraceInformation("Generated keys:");
            Trace.Indent();
            foreach (var id in ids)
                Trace.TraceInformation(id);
            Trace.Unindent();

        }
    }
}
