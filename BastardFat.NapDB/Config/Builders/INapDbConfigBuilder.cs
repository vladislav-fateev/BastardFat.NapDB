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

        IDataSetConfigBuilder<TDb, TKey, TEntity> ConfigureDataSet<TEntity>(Expression<Func<TDb, IDataSet<TEntity, TKey>>> set)
            where TEntity : class, IEntity<TKey>, new();

    }

    public interface IDataSetConfigBuilder<TDb, TKey, TEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseFolderName(string name);

        IDataSetConfigBuilder<TDb, TKey, TEntity> MakeReadonly();

        IDataSetConfigBuilder<TDb, TKey, TEntity> AllowWrite();

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseCustomSerializer(IEntitySerializer<TKey> serializer);

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseCustomReader(IFileReader reader);

        IDataSetConfigBuilder<TDb, TKey, TEntity> UseCustomNameResolver(IFileNameResolver<TKey> resolver);

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> ConfigureProperty<TProp>(Expression<Func<TEntity, TProp>> property);

        INapDbConfigBuilder<TDb, TKey> BuildDataSetConfiguration();

    }

    public interface IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {

        IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> HasOneReferenceTo<TRefEntity>(Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referenceTo)
            where TRefEntity : class, IEntity<TKey>, new();

        IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> HasManyReferencesTo<TRefEntity>(Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referencesTo)
            where TRefEntity : class, IEntity<TKey>, new();

        IBackRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> IsBackReferencesTo<TRefEntity>(Expression<Func<TDb, IDataSet<TRefEntity, TKey>>> referencesTo)
            where TRefEntity : class, IEntity<TKey>, new();

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> Index(string indexName);

        IDataSetConfigBuilder<TDb, TKey, TEntity> BuildPropertyConfiguration();

    }

    public interface IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TRefEntity : class, IEntity<TKey>, new()
    {

        IEntityPropertyConfigBuilder<TDb, TKey, TEntity, TProp> BuildRelationConfiguration();

    }

    public interface IOneRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> : IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TRefEntity : class, IEntity<TKey>, new()
    {

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingForeignKey(Expression<Func<TEntity, TKey>> foreignKey);
        
    }

    public interface IManyRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> : IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TRefEntity : class, IEntity<TKey>, new()
    {

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingForeignKeys(Expression<Func<TEntity, IEnumerable<TKey>>> foreignKeys);

    }

    public interface IBackRelationsConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> : IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity>
        where TDb : INapDb<TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TRefEntity : class, IEntity<TKey>, new()
    {

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingOneForeignKey(Expression<Func<TRefEntity, TKey>> foreignKey);

        IRelationConfigBuilder<TDb, TKey, TEntity, TProp, TRefEntity> UsingManyForeignKeys(Expression<Func<TRefEntity, IEnumerable<TKey>>> foreignKeys);

    }
}