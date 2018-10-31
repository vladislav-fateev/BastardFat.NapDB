using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Caching;
using BastardFat.NapDB.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.FileSystem
{
    public class NapDbFileIO<TEntity, TKey> : INapDbFileIO<TEntity, TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
    {
        private readonly INapDbSerializer<TKey> _serializer;
        private readonly INapDbCache<TEntity, TKey> _cache;
        private readonly INapDb<TKey> _db;
        private readonly INapDbSet _set;
        private readonly TKey _metaId;
        private DirectoryInfo _directory;

        public NapDbFileIO(
            INapDbSerializer<TKey> serializer,
            INapDbCache<TEntity, TKey> cache,
            INapDb<TKey> db, 
            INapDbSet set, 
            TKey metaId)
        {
            _serializer = serializer;
            _cache = cache;
            _db = db;
            _set = set;
            _metaId = metaId;
            NapDbInitializationException.Wrap(_db.GetRootDirectory(), Initialize);
        }

        private void Initialize()
        {
            var path = Path.Combine(_db.GetRootDirectory(), _set.Name);
            if (!Directory.Exists(path))
                _directory = Directory.CreateDirectory(path);
            else
                _directory = new DirectoryInfo(path);
        }

        public IEnumerable<TKey> GetAllIds()
        {
            return FindAllFiles().Select(x => _serializer.ParseId(x.Name.Split('.')[0]));
        }

        public TEntity Read(TKey id)
        {
            var file = FindFile(id);
            if (file == null)
                return null;
            if (_cache.GetCachedSignature(id) == SignatureFromFile(file))
                return _cache.GetCachedValue(id);
            var bytes = File.ReadAllBytes(FindFile(id).FullName);
            var entity = _serializer.Deserialize<TEntity>(bytes);
            return _cache.SaveToCache(entity, _serializer.GetSignature(bytes));
        }

        public void Remove(TKey id)
        {
            var file = FindFile(id);
            if (file == null)
                throw new NapDbException(_db.GetRootDirectory(), $"Database set {_set.Name} doesn't contains entity with id = {_serializer.StringifyId(id)}");
            file.Delete();
            _cache.RemoveCached(id);
        }

        public TEntity Write(TEntity entity)
        {
            var serialized = _serializer.Serialize(entity);
            var signature = _serializer.GetSignature(serialized);

            var file = FindFile(entity.Id);
            if(file != null)
            {
                var oldSig = SignatureFromFile(file);
                if (oldSig != signature)
                    file.Delete();
            }

            File.WriteAllBytes(FileNameForEntity(entity, signature), serialized);
            return _cache.SaveToCache(entity, signature);
        }


        private FileInfo FindFile(TKey id)
        {
            return _directory.GetFiles($"{_serializer.StringifyId(id)}.*.{_serializer.FileExtension}").FirstOrDefault();
        }

        private string FileNameForEntity(TEntity entity, string signature)
        {
            return $"{_serializer.StringifyId(entity.Id)}.{signature}.{_serializer.FileExtension}";
        }

        private IEnumerable<FileInfo> FindAllFiles()
        {
            return _directory.GetFiles($"*.*.{_serializer.FileExtension}").Where(x => x.Name.StartsWith($"{_serializer.StringifyId(_metaId)}."));
        }

        private TKey IdFromFile(FileInfo file)
        {
            return _serializer.ParseId(file.Name.Split('.')[0]);
        }

        private string SignatureFromFile(FileInfo file)
        {
            return file.Name.Split('.')[1];
        }
    }
}
