using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Metadatas
{
    public class GuidMetadata : IDataSetMeta<string>
    {
        public virtual string Id { get; set; }

        public virtual string GetNextId()
        {
            return Guid.NewGuid().ToString();
        }

        public virtual string GetNullId()
        {
            return GetDefaultId();
        }

        public static string GetDefaultId()
        {
            return Guid.Empty.ToString();
        }
    }
}
