using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.FileSystem
{
    public class FileReader : IFileReader
    {
        private readonly FileReaderFolderCreationMode _creationMode;

        public FileReader(FileReaderFolderCreationMode creationMode)
        {
            _creationMode = creationMode;
        }

        public virtual byte[] Read(string folderPath, string name)
        {
            if (_creationMode.HasFlag(FileReaderFolderCreationMode.CreateWhenRead) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return File.ReadAllBytes(Path.Combine(folderPath, name));
        }

        public virtual void Write(string folderPath, string name, byte[] content)
        {
            if (_creationMode.HasFlag(FileReaderFolderCreationMode.CreateWhenWrite) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            File.WriteAllBytes(Path.Combine(folderPath, name), content);
        }

        public virtual IEnumerable<string> Search(string folderPath, string pattern)
        {
            if (_creationMode.HasFlag(FileReaderFolderCreationMode.CreateWhenSearch) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return new DirectoryInfo(folderPath).EnumerateFiles(pattern).Select(f => f.Name);
        }

        public virtual void Remove(string folderPath, string name)
        {
            File.Delete(Path.Combine(folderPath, name));
        }

        public virtual bool HasChangedSince(string folderPath, DateTime since)
        {
            return new DirectoryInfo(folderPath).LastWriteTime > since;
        }
    }
}
