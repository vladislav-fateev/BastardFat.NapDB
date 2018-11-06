using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Locking
{
    public class MutexLocker : ILocker, IDisposable
    {
        private readonly Mutex _mutex;
        private readonly int _millisecondsTimeout;
        private bool _isLocked;

        public virtual bool IsLocked  => _isLocked;

        public MutexLocker(string name) : this(name, Timeout.Infinite) { }

        public MutexLocker(string name, int millisecondsTimeout)
        {
            var allowEveryoneRule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow);

            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            _mutex = new Mutex(false, $"Global\\{name}.Mutex", out _, securitySettings);
            _millisecondsTimeout = millisecondsTimeout;
            _isLocked = false;
        }

        public virtual void Dispose()
        {
            if (_isLocked)
                _mutex.ReleaseMutex();
            _mutex.Dispose();
        }
        public virtual void Lock()
        {
            _isLocked = _mutex.WaitOne(_millisecondsTimeout, false);
            if (!_isLocked)
                throw new NapDbException("Timeout waiting for NapDb mutex handle");
        }
        public virtual void Unlock()
        {
            if (_isLocked)
                _mutex.ReleaseMutex();
        }
    }
}
