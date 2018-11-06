using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Metadatas
{
    public class Int32IncrementMetadata : IDataSetMeta<int>
    {
        public virtual int Id { get; set; }

        public virtual int Current { get; set; } = 0;

        public virtual int GetNextId()
        {
            Current++;
            return Current;
        }

        public virtual int GetNullId()
        {
            return 0;
        }
    }
}
