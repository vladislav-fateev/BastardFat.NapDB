namespace BastardFat.NapDB.Abstractions
{
    public interface IDataSetMeta<TKey> : IEntity<TKey>
    {
        TKey GetNextId();
        TKey GetNullId();
    }

}
