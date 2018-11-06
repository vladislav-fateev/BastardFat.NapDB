using BastardFat.NapDB.Abstractions;

namespace BastardFat.NapDB.Config.Builders
{
    public interface IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TRefEntity : class, IEntity<TKey>, new()
    {

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> BuildRelationConfiguration();

    }
}