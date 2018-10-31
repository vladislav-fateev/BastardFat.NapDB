using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Config
{
    internal static class GlobalDefaults
    {
        public static IEntitySerializer<TKey> CreateDefaultSerializer<TKey>()
        {
            return null;
        }
        public static IFileReader CreateDefaultReader()
        {
            return null;
        }

        public static Type CreateDefaultMeta<TKey>()
        {
            return null;
        }
    }
}
