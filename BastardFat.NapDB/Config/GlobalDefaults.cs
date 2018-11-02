using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Implementation;
using BastardFat.NapDB.Serializers;
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
            return new DefaultXmlSerializer<TKey>();
        }
        public static IFileReader CreateDefaultReader()
        {
            return new FileReader(FileReaderFolderCreationMode.CreateWhenWrite | FileReaderFolderCreationMode.CreateWhenSearch);
        }

        internal static IFileNameResolver<TKey> CreateDefaultNameResolver<TKey>()
        {
            return new FileNameResolver<TKey>();
        }
    }
}
