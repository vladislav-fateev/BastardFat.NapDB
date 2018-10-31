using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface IFileReader
    {
        byte[] Read(string fileName);

        void Write(string fileName, byte[] content);

        IEnumerable<string> Search(string pattern);
    }
}
