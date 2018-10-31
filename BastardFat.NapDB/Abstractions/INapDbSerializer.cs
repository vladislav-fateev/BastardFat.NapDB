namespace BastardFat.NapDB.Abstractions
{
    public interface IEntitySerializer<TKey>
    {
        byte[] Serialize<TEntity>(TEntity model) 
            where TEntity : class, IEntity<TKey>, new();

        TEntity Deserialize<TEntity>(byte[] content) 
            where TEntity : class, IEntity<TKey>, new();

        string GetSignature(byte[] content);

        string StringifyId(TKey key);
        TKey ParseId(string key);

        string FileExtension { get; }
    }

}
