using BastardFat.NapDB.Abstractions;
using System;
using System.Linq.Expressions;

namespace BastardFat.NapDB.Config.Builders
{
    public interface IDataSetConfigBuilder<TDb, TKey, TEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseFolderName(string name);

        IDataSetConfigBuilder<TDb, TKey, TEntity> EnableCaching();

        IDataSetConfigBuilder<TDb, TKey, TEntity> DisableCaching();

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseCustomSerializer(IEntitySerializer<TKey> serializer);

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseCustomReader(IFileReader reader);

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseCustomNameResolver(IFileNameResolver<TKey> resolver);

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> ConfigureProperty<TProp>(Expression<Func<TEntity, TProp>> property);

        INapDbConfigBuilder<TDb, TKey> BuildDataSetConfiguration();

    }
}