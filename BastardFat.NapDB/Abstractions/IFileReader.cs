using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface IFileReader
    {
        byte[] Read(string folderPath, string name);

        void Write(string folderPath, string name, byte[] content);

        void Remove(string folderPath, string name);

        IEnumerable<string> Search(string folderPath, string pattern);
    }
}
