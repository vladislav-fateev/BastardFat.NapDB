namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbMeta<TKey> : INapDbEntity<TKey>
    {
        TKey GetNextId();
        TKey GetMetaId();
    }

}
