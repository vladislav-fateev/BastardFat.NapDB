using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Exceptions
{
    [Serializable]
    public class NapDbException : Exception
    {
        public string DatabaseRoot { get; }

        public NapDbException(string databaseRoot, string message) : base(message) { DatabaseRoot = databaseRoot; }
        public NapDbException(string databaseRoot, string message, Exception inner) : base(message, inner) { DatabaseRoot = databaseRoot; }
        protected NapDbException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
