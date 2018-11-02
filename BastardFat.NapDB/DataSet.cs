using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Abstractions.DataStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB
{
    public abstract class DataSet<TKey> : IDataSet<TKey>
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
        public bool IsReadOnly { get; internal set; }

        public abstract int Count();

        public abstract Type GetEntityType();
    }

    public sealed class DataSet<TEntity, TMeta, TKey> : DataSet<TKey>, IDataSet<TEntity, TMeta, TKey>
         where TEntity : class, IEntity<TKey>, new()
         where TMeta : class, IDataSetMeta<TKey>, new()
    {
        internal DataSet(INapDb<TKey> db, string name) : base(db, name)
        {
            _nullKey = new TMeta().GetNullId();
        }

        private string FolderPath => Database.GetRootDirectory() + FolderName;
        private readonly TKey _nullKey;
        private string _nullKeyStr => NameResolver.StringifyKey(_nullKey);


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
        private IEnumerable<TKey> GetAllIds()
        {

            var pattern = NameResolver.GetSearchPattern(new FileNameSegments());
            var filenames = Reader.Search(FolderPath, pattern);
            var ids = filenames
                .Select(name => NameResolver.GetSegments(name, Array.Empty<string>()).Key)
                .Where(x => x != _nullKeyStr)
                .Select(x => NameResolver.ParseKey(x));

            return ids;
        }
        public TEntity Find(TKey id)
        {
            if (id.Equals(_nullKey))
                return null;
            string pattern = GetPatternForKey(id);
            var filename = Reader.Search(FolderPath, pattern).FirstOrDefault();
            if (filename == null)
                return null;
            var bytes = Reader.Read(FolderPath, filename);
            var entity = Serializer.Deserialize<TEntity>(bytes);
            return entity;
        }
        public IEnumerable<TEntity> Find(IEnumerable<TKey> ids)
        {
            return ids.Select(Find);
        }
        #endregion



        #region Writing
        public TEntity Save(TEntity entity)
        {
            if (entity.Id.Equals(_nullKey))
            {
                var meta = GetMetaObject();
                entity.Id = meta.GetNextId();
                SaveMetaObject(meta);
            }
            return WriteEntity(entity);
        }
        public void Delete(TKey id)
        {
            var pattern = GetPatternForKey(id);
            var filename = Reader.Search(FolderPath, pattern).FirstOrDefault();
            if (filename == null)
                return;
            Reader.Remove(FolderPath, filename);
        }
        #endregion



        #region Meta object
        public TMeta GetMetaObject()
        {
            var pattern = GetPatternForKey(_nullKey);
            var filename = Reader.Search(FolderPath, pattern).FirstOrDefault();
            if (filename == null)
                return SaveMetaObject(new TMeta());
            var bytes = Reader.Read(FolderPath, filename);
            return Serializer.Deserialize<TMeta>(bytes);
        }
        public TMeta SaveMetaObject(TMeta meta)
        {
            meta.Id = _nullKey;
            return WriteEntity(meta);
        }
        #endregion



        #region Private methods
        private T WriteEntity<T>(T entity)
            where T : class, IEntity<TKey>, new()
        {
            var bytes = Serializer.Serialize(entity);
            var sig = Serializer.GetSignature(bytes);
            var keystr = NameResolver.StringifyKey(entity.Id);
            var segments = new FileNameSegments { Key = keystr, Signature = sig };
            var filename = NameResolver.GetFilename(segments);
            var pattern = NameResolver.GetSearchPattern(new FileNameSegments { Key = keystr });
            var existingFilenames = Reader.Search(FolderPath, pattern);
            foreach (var existing in existingFilenames)
                Reader.Remove(FolderPath, existing);
            Reader.Write(FolderPath, filename, bytes);
            return entity;
        }
        private string GetPatternForKey(TKey id)
        {
            var keystr = NameResolver.StringifyKey(id);
            var pattern = NameResolver.GetSearchPattern(new FileNameSegments { Key = keystr });
            return pattern;
        }
        #endregion



        #region IEnumerable implementation
        public IEnumerator<TEntity> GetEnumerator()
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
