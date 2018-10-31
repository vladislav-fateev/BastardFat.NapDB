namespace BastardFat.NapDB.Abstractions
{
    public interface IEntity<TKey> 
    {
        TKey Id { get; set; }
    }

}
