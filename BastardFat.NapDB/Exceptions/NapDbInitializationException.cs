using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Exceptions
{
    [Serializable]
    public class NapDbInitializationException : NapDbException
    {
        private const string DefaultMessage = "An exception occurred during database initialization, see InnerException for details";
        public static void Wrap(string databaseRoot, Action action)
        {
            try
            {
                action();
            }
            catch (NapDbInitializationException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new NapDbInitializationException(databaseRoot, DefaultMessage, exception);
            }
        }

        public NapDbInitializationException(string databaseRoot, string message) : base(databaseRoot, message) { }
        public NapDbInitializationException(string databaseRoot, string message, Exception inner) : base(databaseRoot, message, inner) { }
        protected NapDbInitializationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
