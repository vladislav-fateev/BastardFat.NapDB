namespace BastardFat.NapDB.Abstractions
{
    public interface INapDb<TKey>
    {
        string GetRootDirectory();
        INapDbSet<TEntity, TKey> Set<TEntity>() where TEntity : class, INapDbEntity<TKey>, new();
        INapDbSet<TEntity, TKey> Set<TEntity>(string name) where TEntity : class, INapDbEntity<TKey>, new();
    }
}
