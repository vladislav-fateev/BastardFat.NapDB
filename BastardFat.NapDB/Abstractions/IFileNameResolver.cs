using BastardFat.NapDB.Abstractions.DataStructs;

namespace BastardFat.NapDB.Abstractions
{
    public interface IFileNameResolver<TKey>
    {
        string GetFilename(FileNameSegments segments);
        FileNameSegments GetSegments(string filename, string[] additionalSegmentsNames);
        string GetSearchPattern(FileNameSegments segments);


        string StringifyKey(TKey key);
        TKey ParseKey(string keyString);
    }
}
