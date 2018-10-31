using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Access;
using BastardFat.NapDB.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB
{
    public abstract class NapDb<TKey> : INapDb<TKey>
    {
        private readonly INapDbSetCollection<TKey> _setCollection;
        private readonly string _rootDirectory;

        public NapDb(string rootDirectory)
        {
            _setCollection = new NapDbSetCollection<TKey>(this);
            _rootDirectory = rootDirectory;
            NapDbInitializationException.Wrap(_rootDirectory, FillSets);
        }

        public string GetRootDirectory()
        {
            return _rootDirectory;
        }

        public abstract INapDbMeta<TKey> CreateInitialMeta(INapDbSet<TKey> set);
        public abstract INapDbFileIO<TEntity, TKey> CreateEntityFileIO<TEntity>(INapDbSet<TKey> set) where TEntity : class, INapDbEntity<TKey>, new();
        public abstract INapDbFileIO<TEntity, TKey> CreateMetaFileIO<TEntity>(INapDbSet<TKey> set) where TEntity : class, INapDbEntity<TKey>, new();
        public abstract TKey GetMetaObjectId(INapDbSet<TKey> set);
        public abstract INapDbSerializer<TKey> GetFileSerializer(INapDbSet<TKey> set);
        public abstract INapDbProxier GetProxier(INapDbSet<TKey> set);

        public INapDbSet<TEntity, TKey> Set<TEntity>(string name)
            where TEntity : class, INapDbEntity<TKey>, new()
        {
            return _setCollection.GetSetByEntityTypeAndName<TEntity>(name);
        }

        public INapDbSet<TEntity, TKey> Set<TEntity>()
            where TEntity : class, INapDbEntity<TKey>, new()
        {
            return _setCollection.GetSetByEntityType<TEntity>();
        }

        private void FillSets()
        {
            var props = GetType().GetProperties();

            foreach (var prop in props)
            {
                var iface = prop.PropertyType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(INapDbSet<,>));
                if (iface == null)
                    continue;

                if (iface.GenericTypeArguments[1] != typeof(TKey))
                    throw new NapDbInitializationException(GetRootDirectory(), $"Database set \"{prop.Name}\" key type must be {typeof(TKey).Name}");

                if (prop.PropertyType.GetConstructor(Type.EmptyTypes) == null)
                    throw new NapDbInitializationException(GetRootDirectory(), $"Database set \"{prop.Name}\" of type \"{prop.PropertyType.FullName}\" must contains parameterless constructor");

                var set = Activator.CreateInstance(prop.PropertyType);

                GetType()
                    .GetMethod(nameof(InitializeDbSet), BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(iface.GenericTypeArguments[0])
                    .Invoke(this, new[] { set, prop });
            }
        }

        private void InitializeDbSet<TEntity>(INapDbSet<TEntity, TKey> set, PropertyInfo prop)
            where TEntity : class, INapDbEntity<TKey>, new()
        {
            set.Initialize(this, prop.Name);
            _setCollection.Add(set);
            prop.SetValue(this, set);
        }
    }
}
