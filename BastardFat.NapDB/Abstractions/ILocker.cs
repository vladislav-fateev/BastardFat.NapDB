using System;
using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface ILocker
    {
        void Lock();
        void Unlock();
        bool IsLocked { get; }
    }
}
