using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Logging.Profiling
{
    public class Profiler
    {
        [ThreadStatic]
        private static Stopwatch? _current;

        public static ProfilerScope Sample(string name)
        {
            return new ProfilerScope(name);
        }
    }
}
