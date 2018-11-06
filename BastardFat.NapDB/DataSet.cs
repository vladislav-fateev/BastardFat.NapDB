using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Abstractions.DataStructs;
using BastardFat.NapDB.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB
{
    internal abstract class DataSet<TKey> : IDataSet<TKey>
    {
        internal DataSet(INapDb<TKey> db, string name)
        {
            Database = db;
            Name = name;
        }

        public INapDb<TKey> Database { get; }
        public string Name { get; }
        public string FolderName { get; internal set; }
        public IEntitySerializer<TKey> Serializer { get; internal set; }
        public IFileReader Reader { get; internal set; }
        public IFileNameResolver<TKey> NameResolver { get; internal set; }
        public bool EnableCaching { get; internal set; }

        public abstract int Count();

        public abstract Type GetEntityType();

        public abstract void MetaCacheLock();

        public abstract void MetaCacheUnlock();
    }

    internal class DataSet<TEntity, TMeta, TKey> : DataSet<TKey>, IDataSet<TEntity, TMeta, TKey>
         where TEntity : class, IEntity<TKey>, new()
         where TMeta : class, IDataSetMeta<TKey>, new()
    {
        internal DataSet(INapDb<TKey> db, string name) : base(db, name)
        {
            _nullKey = new TMeta().GetNullId();
            _cache = new MemoryCacheFactory().GetMemoryCache<TKey, TEntity>(Name);
        }

        private readonly TKey _nullKey;

        private string FolderPath => Database.GetRootDirectory() + FolderName;
        private string _nullKeyStr => NameResolver.StringifyKey(_nullKey);
        private MemoryCache<TKey, TEntity> _cache;

        #region Overrides
        public override Type GetEntityType()
        {
            return typeof(TEntity);
        }
        public override int Count()
        {
            return GetAllIds().Count();
        }
        #endregion



        #region Reading
        public virtual TEntity Find(TKey id)
        {
            if (id.Equals(_nullKey))
                return null;
            string pattern = GetPatternForKey(id);
            var filename = SearchNames(FolderPath, pattern).FirstOrDefault();
            if (filename == null)
            {
                if (EnableCaching)
                    _cache.RemoveCached(id);
                return null;
            }
            string signature = null;
            if (EnableCaching)
            {
                signature = NameResolver.GetSegments(filename, Array.Empty<string>()).Signature;
                if (_cache.IsCached(id, signature))
                    return _cache.GetCachedValue(id);
            }
            var bytes = Reader.Read(FolderPath, filename);
            var entity = Serializer.Deserialize<TEntity>(bytes);
            entity = Proxy(entity);
            if (EnableCaching)
                _cache.CacheValue(entity, signature);
            return entity;
        }
        public virtual IEnumerable<TEntity> Find(IEnumerable<TKey> ids)
        {
            return ids.Select(Find);
        }
        #endregion



        #region Writing
        public virtual TEntity Save(TEntity entity)
        {
            if (entity.Id.Equals(_nullKey))
            {
                var meta = GetMetaObject();
                entity.Id = meta.GetNextId();
                SaveMetaObject(meta);
            }
            entity = WriteEntity(entity, out string signature);
            entity = Proxy(entity);
            if (EnableCaching)
            {
                if (!_cache.IsCached(entity.Id, signature))
                    entity = _cache.CacheValue(entity, signature);
            }
            return entity;
        }
        public virtual void Delete(TKey id)
        {
            if (EnableCaching)
                _cache.RemoveCached(id);
            var pattern = GetPatternForKey(id);
            var filename = SearchNames(FolderPath, pattern).FirstOrDefault();
            if (filename == null)
                return;
            Reader.Remove(FolderPath, filename);
        }
        #endregion



        #region Meta object
        private TMeta _meta;
        private string _metaSignature;
        private bool _dbLocked;
        private DateTime _metaLockedAt;
        public virtual TMeta GetMetaObject()
        {
            if (_dbLocked && EnableCaching)
            {
                if (_meta == null)
                    _meta = new TMeta();
                return _meta;
            }

            var pattern = GetPatternForKey(_nullKey);
            var filename = SearchNames(FolderPath, pattern).FirstOrDefault();
            if (filename == null)
                return _meta ?? SaveMetaObject(new TMeta());
            string signature = null;
            if (EnableCaching)
            {
                signature = NameResolver.GetSegments(filename, Array.Empty<string>()).Signature;
                if (signature == _metaSignature)
                    return _meta;
            }
            var bytes = Reader.Read(FolderPath, filename);
            _meta = Serializer.Deserialize<TMeta>(bytes);
            if (EnableCaching)
                _metaSignature = signature;
            return _meta;
        }
        public virtual TMeta SaveMetaObject(TMeta meta)
        {
            meta.Id = _nullKey;
            if (_dbLocked && EnableCaching)
            {
                _meta = meta;
                return _meta;
            }
            _meta = WriteEntity(meta, out _metaSignature);
            return _meta;
        }
        public override void MetaCacheLock()
        {
            _meta = GetMetaObject();
            _dbLocked = true;
            _metaLockedAt = DateTime.Now;
        }
        public override void MetaCacheUnlock()
        {
            _dbLocked = false;
            _meta = SaveMetaObject(_meta);
        }
        #endregion



        #region Private methods

        private IEnumerable<string> SearchNames(string folderPath, string pattern)
        {
            if (!EnableCaching)
                return Reader.Search(folderPath, pattern);

            if (_cache.GetCachedSearchResult(folderPath, pattern, out CachedSearchResult cached))
                if ((_dbLocked && _metaLockedAt < cached.CachedAt) || !Reader.HasChangedSince(folderPath, cached.CachedAt))
                    return cached.Result;

            var result = Reader.Search(folderPath, pattern);
            _cache.CacheSearchResult(folderPath, pattern, result);
            return result;
        }
        private IEnumerable<TKey> GetAllIds()
        {
            var pattern = NameResolver.GetSearchPattern(new FileNameSegments());
            var filenames = SearchNames(FolderPath, pattern);
            var ids = filenames
                .Select(name => NameResolver.GetSegments(name, Array.Empty<string>()).Key)
                .Where(x => x != _nullKeyStr)
                .Select(x => NameResolver.ParseKey(x));

            return ids;
        }
        private T WriteEntity<T>(T entity, out string signature)
            where T : class, IEntity<TKey>, new()
        {
            var bytes = Serializer.Serialize(entity);
            signature = Serializer.GetSignature(bytes);
            var keystr = NameResolver.StringifyKey(entity.Id);
            var segments = new FileNameSegments { Key = keystr, Signature = signature };

            var forSamePattern = NameResolver.GetSearchPattern(segments);
            var sameFilename = SearchNames(FolderPath, forSamePattern).FirstOrDefault();
            if (sameFilename == null)
            {
                var filename = NameResolver.GetFilename(segments);
                var pattern = NameResolver.GetSearchPattern(new FileNameSegments { Key = keystr });
                var existingFilenames = SearchNames(FolderPath, pattern);
                foreach (var existing in existingFilenames)
                    Reader.Remove(FolderPath, existing);
                Reader.Write(FolderPath, filename, bytes);
            }
            return entity;
        }
        private string GetPatternForKey(TKey id)
        {
            var keystr = NameResolver.StringifyKey(id);
            var pattern = NameResolver.GetSearchPattern(new FileNameSegments { Key = keystr });
            return pattern;
        }
        private TEntity Proxy(TEntity entity)
        {
            // TODO: Implement Proxy
            return entity;
        }
        #endregion



        #region IEnumerable implementation
        public virtual IEnumerator<TEntity> GetEnumerator()
        {
            return GetAllIds().Select(Find).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
