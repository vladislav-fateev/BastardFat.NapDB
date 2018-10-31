using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Caching
{
    public class NapDbCachedEntity<TEntity, TKey>
        where TEntity : class, INapDbEntity<TKey>, new()
    {
        public TEntity Entity { get; set; }
        public string Signature { get; set; }
        public DateTime CachedAt { get; set; }
    }
}
