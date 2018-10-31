using System.Collections.Generic;

namespace BastardFat.NapDB.Abstractions
{
    public interface INapDbProxier
    {
        bool IsProxiedObject<T>(T obj)
            where T : class, new();

        T CreateNewProxiedObject<T>()
            where T : class, new();

        T Proxy<T>(T obj)
            where T : class, new();

        T Unproxy<T>(T obj)
            where T : class, new();
    }
}
