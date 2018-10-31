namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbSerializer<TKey>
    {
        byte[] Serialize<TEntity>(TEntity model) 
            where TEntity : class, INapDbEntity<TKey>, new();

        TEntity Deserialize<TEntity>(byte[] content) 
            where TEntity : class, INapDbEntity<TKey>, new();

        string GetSignature(byte[] content);

        string StringifyId(TKey key);
        TKey ParseId(string key);

        string FileExtension { get; }
    }

}
