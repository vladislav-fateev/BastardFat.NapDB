﻿using System;
using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface IDataSet
    {
        string Name { get; }
        string FolderName { get; }
        int Count();
        Type GetEntityType();
    }

    public interface IDataSet<TKey>: IDataSet
    {
        INapDb<TKey> Database { get; }
    }

    public interface IDataSet<TEntity, TKey> : IDataSet<TKey>, IEnumerable<TEntity> 
        where TEntity : class, IEntity<TKey>, new()
    {
        TEntity Find(TKey id);
        IEnumerable<TEntity> Find(IEnumerable<TKey> ids);
        TEntity Save(TEntity entity);
        IEnumerable<TEntity> WherePropertyIs<TValue>(string propertyName, TValue value);
        void Delete(TKey id);
    }

    public interface IDataSet<TEntity, TMeta, TKey> : IDataSet<TEntity, TKey>
         where TEntity : class, IEntity<TKey>, new()
         where TMeta : class, INapDbMeta<TKey>, new()
    {
        TMeta GetMetaObject();
        TMeta SaveMetaObject(TMeta meta);
    }

}