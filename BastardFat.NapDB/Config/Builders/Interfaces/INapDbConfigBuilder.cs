using BastardFat.NapDB.Abstractions;
using System;
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
        INapDbConfigBuilder<TDb, TKey> UseCustomLocker(ILocker locker);

        IDataSetConfigBuilder<TDb, TKey, TEntity> ConfigureDataSet<TEntity>(Expression<Func<TDb, IDataSet<TEntity, TKey>>> set)
            where TEntity : class, IEntity<TKey>, new();

    }
}