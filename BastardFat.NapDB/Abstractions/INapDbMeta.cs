namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbMeta<TKey> : IEntity<TKey>
    {
        TKey GetNextId();
        TKey GetMetaId();
    }

}
