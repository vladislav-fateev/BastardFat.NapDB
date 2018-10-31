using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.FileSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB
{
    public class NapDbSet<TEntity, TMeta, TKey> : INapDbSet<TEntity, TMeta, TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
        where TMeta : class, INapDbMeta<TKey>, new()
    {
        private readonly INapDbFileIO<TEntity, TKey> _entityIO;
        private readonly INapDbFileIO<TMeta, TKey> _metaIO;
        private readonly TKey _metaId;
        public NapDbSet(
            INapDbFileIO<TEntity, TKey> entityIO,
            INapDbFileIO<TMeta, TKey> metaIO)
        {
            _entityIO = entityIO;
            _metaIO = metaIO;
            _metaId = new TMeta().GetMetaId();
        }

        public void Initialize(INapDb<TKey> database, string name)
        {
            Database = database;
            Name = name;
        }

        public string Name { get; private set; }

        public INapDb<TKey> Database { get; private set; }


        public TMeta GetMetaObject()
        {
            var meta = _metaIO.Read(_metaId);
            if (meta == null)
            {
                meta = new TMeta();
                meta = SaveMetaObject(meta);
            }
            return meta;
        }

        public TMeta SaveMetaObject(TMeta meta)
        {
            return _metaIO.Write(meta);
        }

        private IEnumerable<TEntity> All()
        {
            return _entityIO.GetAllIds().Select(id => _entityIO.Read(id));
        }

        public int Count()
        {
            return _entityIO.GetAllIds().Count();
        }

        public void Delete(TKey id)
        {
            _entityIO.Remove(id);
        }

        public TEntity Find(TKey id)
        {
            return _entityIO.Read(id);
        }

        public IEnumerable<TEntity> Find(IEnumerable<TKey> ids)
        {
            return ids.Select(id => _entityIO.Read(id));
        }

        public TEntity Save(TEntity entity)
        {
            if (entity.Id.Equals(default(TKey)))
                entity.Id = GetMetaObject().GetNextId();
            return _entityIO.Write(entity);
        }

        public IEnumerable<TEntity> WherePropertyIs<TValue>(string propertyName, TValue value)
        {
            return All()
                .Where(x => x
                    .GetType()
                    .GetProperty(propertyName)
                    .GetValue(x)
                    .Equals(value));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return All().GetEnumerator();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
