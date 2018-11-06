using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Proxy
{
    internal class ProxyGeneratorFactory
    {
        private Lazy<ProxyGenerator> _generator = new Lazy<ProxyGenerator>(() => new ProxyGenerator());
        public ProxyGenerator GetProxyGenerator()
        {
            return _generator.Value;
        }
    }
}
