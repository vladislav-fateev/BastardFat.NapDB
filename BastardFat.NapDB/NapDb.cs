using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB
{
    public abstract class NapDb<TKey> : INapDb<TKey>
    {
        // Will be removed
        public abstract IEnumerable<IDataSet<TKey>> AllDataSets();
        public abstract IDataSet<TKey> DataSet(string name);
        public abstract string GetRootDirectory();

        IDataSet<TEntity, TKey> INapDb<TKey>.DataSet<TEntity>()
        {
            throw new NotImplementedException();
        }

        IDataSet<TEntity, TKey> INapDb<TKey>.DataSet<TEntity>(string name)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// JUST DRAFT!!! Will be removed
    /// </summary>
    public class MutexWrapper : IDisposable
    {
        private System.Threading.Mutex mutex = new System.Threading.Mutex(false, "testmutex");
        private MutexWrapper()
        {
            mutex.WaitOne();
        }
        public void Dispose()
        {
            mutex.ReleaseMutex();
        }

        public static MutexWrapper Lock()
        {
            return new MutexWrapper();
        }
    }
}
