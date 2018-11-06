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
using BastardFat.NapDB.Tests.Support;
using BastardFat.NapDB.Locking;

namespace BastardFat.NapDB.Tests
{
    [TestClass]
    public class PerformanceTest
    {
        public const string Path = @"C:\dev\BastardFat.NapDB\_databases\PerformanceTestDb";
        public const string EntityFolder = "_caching_performance";

        [TestMethod]
        public void CachingPerformance()
        {
            var db = Measure(2, false, out _); // warmup

            Trace.TraceInformation("Without cache");
            db = Measure(100, false, out Stopwatch totalTimeWithoutCache);

            Trace.TraceInformation("Total running time is {0} ms.", totalTimeWithoutCache.ElapsedMilliseconds);
            var totalDiskTimeWithoutCache = PrintMeasures(db.PerformanceFileReader.Measurer, "Total file reader time");
            var totalSerializerTimeWithoutCache = PrintMeasures(db.PerformanceXmlSerializer.Measurer, "Total serializer time");

            var p1 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "WithoutCache");
            if (Directory.Exists(p1))
                Directory.Delete(p1, true);
            Directory.Move(Path, p1);


            Trace.TraceInformation("================");


            Trace.TraceInformation("With cache");
            db = Measure(100, true, out Stopwatch totalTimeWithCache);

            Trace.TraceInformation("Total running time is {0} ms.", totalTimeWithCache.ElapsedMilliseconds);
            var totalDiskTimeWithCache = PrintMeasures(db.PerformanceFileReader.Measurer, "Total file reader time");
            var totalSerializerTimeWithCache = PrintMeasures(db.PerformanceXmlSerializer.Measurer, "Total serializer time");

            var p2 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "WithCache");
            if (Directory.Exists(p2))
                Directory.Delete(p2, true);
            Directory.Move(Path, p2);


            Assert.IsTrue(totalDiskTimeWithCache < totalDiskTimeWithoutCache * 0.7);
            Assert.IsTrue(totalSerializerTimeWithCache < totalSerializerTimeWithoutCache * 0.7);
            Assert.IsTrue(totalTimeWithCache.ElapsedMilliseconds < totalTimeWithoutCache.ElapsedMilliseconds * 0.7);

            p1 = System.IO.Path.Combine(p1, EntityFolder);
            p2 = System.IO.Path.Combine(p2, EntityFolder);
            var files1 = Directory.EnumerateFiles(p1).ToArray();
            var files2 = Directory.EnumerateFiles(p2).ToArray();

            Assert.AreEqual(files1.Length, files2.Length);

            foreach (var file1 in files1)
            {
                var file2 = System.IO.Path.Combine(p2, System.IO.Path.GetFileName(file1));
                Assert.AreEqual(File.ReadAllText(file1), File.ReadAllText(file2));
            }

            Trace.TraceInformation("Folders '{0}' and '{1}' are identical", p1, p2);
        }

        private static long PrintMeasures(Measurer measurer, string message)
        {
            long totalMeasurerTime = measurer.Measures.Sum(x => x.Value.Time.ElapsedMilliseconds);
            Trace.Indent();
            foreach (var measure in measurer.Measures)
                Trace.TraceInformation("Measure '{0}': {1} times in {2} ms ({3} ticks per time)", measure.Key, measure.Value.Calls, measure.Value.Time.ElapsedMilliseconds, measure.Value.Time.ElapsedTicks / measure.Value.Calls);
            Trace.Unindent();
            Trace.TraceInformation("{1} is {0} ms.", totalMeasurerTime, message);

            return totalMeasurerTime;
        }

        private PerformanceTestDb Measure(int count, bool useCache, out Stopwatch totalTime)
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);

            totalTime = Stopwatch.StartNew();

            var db = new PerformanceTestDb(useCache);

            LockerWrapper lw = null;
            if (useCache)
                lw = db.BeginLock(); // in real world must be 'using(db.BeginLock()) { . . . }'

            for (int i = 0; i < count; i++)
            {
                db.PerformanceTestDataset.Save(new PerformanceTestEntity
                {
                    Created = DateTime.Today,
                    Data = $"Item { i + 1 }",
                });
            }

            var queryedIds = db.PerformanceTestDataset.Where(x => x.Data.EndsWith("0"));

            var queryResult = db.PerformanceTestDataset.Find(queryedIds.Select(x => x.Id));

            foreach (var item in queryResult)
            {
                item.Created = item.Created + TimeSpan.FromDays(1);
                db.PerformanceTestDataset.Save(item);
            }

            for (int i = count / 2; i < count; i++)
            {
                var item = db.PerformanceTestDataset.Find(i);
                item.Data = item.Data + " (second half)";
                db.PerformanceTestDataset.Save(item);
            }

            var arr = db.PerformanceTestDataset.ToArray();

            foreach (var p in db.PerformanceTestDataset.Take(count))
            {
                db.PerformanceTestDataset.Save(p);
            }

            if (useCache)
                lw?.Dispose();

            totalTime.Stop();

            return db;
        }
    }
}
