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
        public string DatabaseName { get; }

        public NapDbException(string message) : base(message) { }
        public NapDbException(string message, Exception inner) : base(message, inner) { }
        public NapDbException(string databaseName, string message) : base(message) { DatabaseName = databaseName; }
        public NapDbException(string databaseName, string message, Exception inner) : base(message, inner) { DatabaseName = databaseName; }
        protected NapDbException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
