using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Locking
{
    public sealed class LockerWrapper : IDisposable
    {
        private readonly ILocker _locker;
        private readonly Action _onEndLock;

        public LockerWrapper(ILocker locker, Action onBeginLock, Action onEndLock)
        {
            _locker = locker;
            _onEndLock = onEndLock;
            _locker.Lock();
            onBeginLock();
        }

        public void Dispose()
        {
            _onEndLock();
            _locker.Unlock();
        }
    }
}
