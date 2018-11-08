using BastardFat.NapDB.FileSystem;
using System;
using System.Collections.Generic;

namespace BastardFat.NapDB.Tests.Performance
{
    public class PerformanceFileReader : FileReader
    {
        public PerformanceFileReader(FileReaderFolderCreationMode creationMode) : base(creationMode)
        {
            Measurer = new PerformanceMeasurer();
        }
        public PerformanceMeasurer Measurer { get; }

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
}
