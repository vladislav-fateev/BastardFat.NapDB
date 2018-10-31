namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbEntity<TKey> 
    {
        TKey Id { get; set; }
    }

}
