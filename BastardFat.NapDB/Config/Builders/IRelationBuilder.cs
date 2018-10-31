using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Config.Builders
{
    public interface INapDbConfigBuilder<TDb, TKey>
        where TDb : INapDb<TKey>
    {

        INapDbConfigBuilder<TDb, TKey> UseRootFolderPath(string path);

        IDataSetConfigBuilder<TDb, TKey, TEntity> ConfigureDataSet<TEntity>(Expression<Func<TDb, INapDbSet<TEntity, TKey>>> set)
            where TEntity : class, INapDbEntity<TKey>, new();

    }

    public interface IDataSetConfigBuilder<TDb, TKey, TEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
    {

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseFolderName(string name);

        IDataSetConfigBuilder<TDb, TKey, TEntity> MakeReadonly();

        IDataSetConfigBuilder<TDb, TKey, TEntity> AllowWrite();

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseCustomSerializer(INapDbSerializer<TKey> serializer);

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseCustomMetadata<TMeta>() where TMeta : INapDbMeta<TKey>, new();

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> ConfigureProperty<TProp>(Expression<Func<TEntity, TProp>> property);

        INapDbConfigBuilder<TDb, TKey> BuildDataSetConfiguration();

    }

    public interface IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>
        where TDb : INapDb<TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
    {

        IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> HasOne<TRefEntity>(Expression<Func<TDb, INapDbSet<TRefEntity, TKey>>> referenceTo)
            where TRefEntity : class, INapDbEntity<TKey>, new();

        IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> HasMany<TRefEntity>(Expression<Func<TDb, INapDbSet<TRefEntity, TKey>>> referencesTo)
            where TRefEntity : class, INapDbEntity<TKey>, new();

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> IgnoredBySerializer();

        IDataSetConfigBuilder<TDb, TKey, TEntity> BuildPropertyConfiguration();

    }

    public interface IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
        where TRefEntity : class, INapDbEntity<TKey>, new()
    {

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> BuildRelationConfiguration();

    }

    public interface IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> : IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
        where TRefEntity : class, INapDbEntity<TKey>, new()
    {

        IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingForeignKey(Expression<Func<TEntity, TKey>> foreignKey);

        IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> WithBackReferences(Expression<Func<TRefEntity, IEnumerable<TEntity>>> backReferences);

    }

    public interface IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> : IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
        where TRefEntity : class, INapDbEntity<TKey>, new()
    {

        IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingForeignKeys(Expression<Func<TEntity, IEnumerable<TKey>>> foreignKeys);

        IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> WithBackReferences(Expression<Func<TRefEntity, IEnumerable<TEntity>>> backReferences);

    }
}