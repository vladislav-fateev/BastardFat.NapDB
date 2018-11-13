using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Config;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Proxy.Interceptors
{
    internal class EntityInterceptor<TKey> : IInterceptor
    {
        private readonly INapDb<TKey> _db;
        private readonly Dictionary<string, EntityPropertyConfiguration<TKey>> _config;

        public EntityInterceptor(INapDb<TKey> db, Dictionary<string, EntityPropertyConfiguration<TKey>> config)
        {
            _db = db;
            _config = config;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            var key = _config.Keys.FirstOrDefault(x => "get_" + x == invocation.Method.Name);

            if (key != null && _config[key].IsReference)
            {
                var reference = _config[key].Reference;
                switch (reference.Kind)
                {
                    case ReferenceKind.OneToMany:
                        invocation.ReturnValue = reference.SourceDataSet.FindObject(
                            (TKey)reference.ForeignKeyProperty.GetValue(invocation.InvocationTarget));
                        break;
                    case ReferenceKind.ManyToMany:
                        break;
                    case ReferenceKind.BackFromOne:
                        IEnumerable<object> enumerable = reference.SourceDataSet
                            .FindAllObjects()
                            .Where(x =>
                               ((TKey)reference.ForeignKeyProperty.GetValue(x)).Equals((invocation.InvocationTarget as IEntity<TKey>).Id));

                        var casted = typeof(Enumerable)
                            .GetMethod("Cast", BindingFlags.Static | BindingFlags.Public)
                            .MakeGenericMethod(reference.SourceDataSet.GetEntityType())
                            .Invoke(null, new object[] { enumerable });

                        invocation.ReturnValue = casted;
                        break;
                    case ReferenceKind.BackFromMany:
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
