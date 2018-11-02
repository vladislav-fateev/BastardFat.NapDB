using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Implementation
{
    public enum FileReaderFolderCreationMode
    {
        DontCreate = 0,
        CreateWhenWrite = 1,
        CreateWhenRead = 2,
        CreateWhenSearch = 4,
    }

    public class FileReader : IFileReader
    {
        private readonly FileReaderFolderCreationMode _creationMode;

        public FileReader(FileReaderFolderCreationMode creationMode)
        {
            _creationMode = creationMode;
        }

        public byte[] Read(string folderPath, string name)
        {
            if (_creationMode.HasFlag(FileReaderFolderCreationMode.CreateWhenRead) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return File.ReadAllBytes(Path.Combine(folderPath, name));
        }

        public void Write(string folderPath, string name, byte[] content)
        {
            if (_creationMode.HasFlag(FileReaderFolderCreationMode.CreateWhenWrite) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            File.WriteAllBytes(Path.Combine(folderPath, name), content);
        }

        public IEnumerable<string> Search(string folderPath, string pattern)
        {
            if (_creationMode.HasFlag(FileReaderFolderCreationMode.CreateWhenSearch) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return new DirectoryInfo(folderPath).EnumerateFiles(pattern).Select(f => f.Name);
        }

        public void Remove(string folderPath, string name)
        {
            File.Delete(Path.Combine(folderPath, name));
        }
    }
}
