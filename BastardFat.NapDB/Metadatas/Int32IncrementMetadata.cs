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
        public int Id { get; set; }

        public int Current { get; set; } = 0;

        public int GetNextId()
        {
            Current++;
            return Current;
        }

        public int GetNullId()
        {
            return 0;
        }
    }
}
