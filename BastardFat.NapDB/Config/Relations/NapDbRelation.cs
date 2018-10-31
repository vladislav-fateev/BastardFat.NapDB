using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Config.Relations
{
    public class NapDbRelation<T>
    {
        public string SetName { get; set; }
        public string PropName { get; set; }
        public Func<T> Injector { get; set; }
    }
}
