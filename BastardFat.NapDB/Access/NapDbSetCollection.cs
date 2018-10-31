using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Access
{
    public class NapDbSetCollection<TKey> : INapDbSetCollection<TKey>
    {
        private Dictionary<Type, List<INapDbSet<TKey>>> _collection = new Dictionary<Type, List<INapDbSet<TKey>>>();
        private readonly INapDb<TKey> _db;

        public NapDbSetCollection(INapDb<TKey> db)
        {
            _db = db;
        }

        public void Add<TEntity>(INapDbSet<TEntity, TKey> set)
            where TEntity : class, INapDbEntity<TKey>, new()
        {
            if (!_collection.ContainsKey(typeof(TEntity)))
                _collection.Add(typeof(TEntity), new List<INapDbSet<TKey>>());

            if (_collection[typeof(TEntity)].Any(x => x.Name == set.Name))
                throw new NapDbInitializationException(_db.GetRootDirectory(), $"Database set named \"{set.Name}\" can not be declared more than once");

            _collection[typeof(TEntity)].Add(set);
        }

        public INapDbSet<TEntity, TKey> GetSetByEntityType<TEntity>()
            where TEntity : class, INapDbEntity<TKey>, new()
        {
            if (!_collection.ContainsKey(typeof(TEntity)))
                return null;

            if (_collection[typeof(TEntity)].Count() > 1)
                throw new NapDbException(_db.GetRootDirectory(), $"Database has multiple sets of type {typeof(TEntity).FullName}");

            return (INapDbSet<TEntity, TKey>) _collection[typeof(TEntity)].FirstOrDefault();
        }

        public INapDbSet<TEntity, TKey> GetSetByEntityTypeAndName<TEntity>(string name)
            where TEntity : class, INapDbEntity<TKey>, new()
        {
            if (!_collection.ContainsKey(typeof(TEntity)))
                return null;

            return (INapDbSet<TEntity, TKey>) _collection[typeof(TEntity)].FirstOrDefault(x => x.Name == name);
        }
    }
}
