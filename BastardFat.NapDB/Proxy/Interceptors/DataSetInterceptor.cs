using BastardFat.NapDB.Abstractions;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Proxy.Interceptors
{
    internal class DataSetInterceptor : IInterceptor
    {
        private readonly Action _beforeCall;

        public DataSetInterceptor(Action beforeCall)
        {
            _beforeCall = beforeCall;
        }
        public void Intercept(IInvocation invocation)
        {
            if(invocation.Method.Name != nameof(IDataSet<int>.GetEntityType))
                _beforeCall();
            invocation.Proceed();
        }
    }
}
