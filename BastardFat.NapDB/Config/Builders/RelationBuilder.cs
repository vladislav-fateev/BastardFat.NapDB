using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Config.Builders
{
    public class RelationBuilder<TDbKey> : IRelationBuilder<TDbKey>
        where TDbKey : INapDb<TDbKey>
    {
        public ISetRelationBuilder<TDbKey, TDestEntity> Set<TDestEntity>(Expression<Func<INapDb<TDbKey>, INapDbSet<TDestEntity, TDbKey>>> set)
            where TDestEntity : class, INapDbEntity<TDbKey>, new()
        {
            return new RelationBuilder<TDbKey, TDestEntity>(set);
        }
    }

    public class RelationBuilder<TDbKey, TDestEntity> : ISetRelationBuilder<TDbKey, TDestEntity>
        where TDbKey : INapDb<TDbKey>
        where TDestEntity : class, INapDbEntity<TDbKey>, new()
    {
        public Expression<Func<INapDb<TDbKey>, INapDbSet<TDestEntity, TDbKey>>> DestSet { get; }

        internal RelationBuilder(Expression<Func<INapDb<TDbKey>, INapDbSet<TDestEntity, TDbKey>>> destSet)
        {
            DestSet = destSet;
        }

        public IOneRelationBuilder<TDbKey, TDestEntity, TSrcEntity> HasOne<TSrcEntity>(Expression<Func<TDestEntity, TSrcEntity>> reference)
            where TSrcEntity : class, INapDbEntity<TDbKey>, new()
        {
            return new RelationBuilder<TDbKey, TDestEntity, TSrcEntity>(reference)
            {
                DestProperty = reference,
                DestSet = DestSet
            };
        }

        public IManyRelationsBuilder<TDbKey, TDestEntity, TSrcEntity> HasMany<TSrcEntity>(Expression<Func<TDestEntity, IEnumerable<TSrcEntity>>> references)
            where TSrcEntity : class, INapDbEntity<TDbKey>, new()
        {
            return new RelationBuilder<TDbKey, TDestEntity, TSrcEntity>(references)
            {
                DestProperties = references,
                DestSet = DestSet
            };
        }
    }

    public class RelationBuilder<TDbKey, TDestEntity, TSrcEntity> : 
        IOneRelationBuilder<TDbKey, TDestEntity, TSrcEntity>,
        IManyRelationsBuilder<TDbKey, TDestEntity, TSrcEntity>,
        IOneReferencedRelationBuilder<TDbKey, TDestEntity, TSrcEntity>,
        IManyReferencedRelationsBuilder<TDbKey, TDestEntity, TSrcEntity>,
        IKeyRelationBuilder<TDbKey, TDestEntity, TSrcEntity>
            where TDbKey : INapDb<TDbKey>
            where TDestEntity : class, INapDbEntity<TDbKey>, new()
            where TSrcEntity : class, INapDbEntity<TDbKey>, new()
    {
        public Expression<Func<INapDb<TDbKey>, INapDbSet<TDestEntity, TDbKey>>> DestSet { get; internal set; }
        public Expression<Func<TDestEntity, TSrcEntity>> DestProperty { get; internal set; }
        public Expression<Func<TDestEntity, IEnumerable<TSrcEntity>>> DestProperties { get; internal set; }
        public Expression<Func<INapDb<TDbKey>, INapDbSet<TSrcEntity, TDbKey>>> SrcSet { get; private set; }
        public Expression<Func<TDestEntity, TDbKey>> ForeignKey { get; private set; }
        public Expression<Func<TDestEntity, IEnumerable<TDbKey>>> ForeignKeys { get; private set; }

        internal RelationBuilder(Expression<Func<TDestEntity, TSrcEntity>> reference)
        {
            DestProperty = reference;
        }

        internal RelationBuilder(Expression<Func<TDestEntity, IEnumerable<TSrcEntity>>> references)
        {
            DestProperties = references;
        }

        public IManyReferencedRelationsBuilder<TDbKey, TDestEntity, TSrcEntity> ReferencesTo(Expression<Func<INapDb<TDbKey>, INapDbSet<TSrcEntity, TDbKey>>> set)
        {
            SrcSet = set;
            return this;
        }

        public IOneReferencedRelationBuilder<TDbKey, TDestEntity, TSrcEntity> ReferenceTo(Expression<Func<INapDb<TDbKey>, INapDbSet<TSrcEntity, TDbKey>>> set)
        {
            SrcSet = set;
            return this;
        }

        public IKeyRelationBuilder<TDbKey, TDestEntity, TSrcEntity> UsingKey(Expression<Func<TDestEntity, TDbKey>> foreignKey)
        {
            ForeignKey = foreignKey;
            return this;
        }

        public IKeyRelationBuilder<TDbKey, TDestEntity, TSrcEntity> UsingKeys(Expression<Func<TDestEntity, IEnumerable<TDbKey>>> foreignKeys)
        {
            ForeignKeys = foreignKeys;
            return this;
        }

        public IRelationBuilder<TDbKey> WithBack(Expression<Func<TSrcEntity, IEnumerable<TDestEntity>>> backReference)
        {
            throw new NotImplementedException();
        }

        public IRelationBuilder<TDbKey> WithoutBack()
        {
            throw new NotImplementedException();
        }
    }
}