using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BastardFat.NapDB.Tests.Performance
{
    public class PerformanceMeasurer
    {
        public Dictionary<string, Measurement> Measures = new Dictionary<string, Measurement>();

        public void Clear() { Measures.Clear(); }

        public T Measure<T>(Func<T> action, [CallerMemberName] string caller = "")
        {
            if (!Measures.ContainsKey(caller))
                Measures[caller] = new Measurement();
            Measures[caller].Calls++;
            Measures[caller].Time.Start();
            var result = action();
            Measures[caller].Time.Stop();
            return result;
        }

        public class Measurement
        {
            public Stopwatch Time { get; set; } = new Stopwatch();
            public int Calls { get; set; } = 0;
        }
    }
}
