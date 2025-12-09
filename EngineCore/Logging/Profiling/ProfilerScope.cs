using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Logging.Profiling
{
    public class ProfilerScope : IDisposable
    {
        private readonly string _name;
        private readonly Stopwatch? _sw;

        public ProfilerScope(string name)
        {
            _name = name;
            _sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _sw?.Stop();
            if (_sw != null)
                Logger.Log($"{_name}: {_sw.ElapsedMilliseconds} ms", LogLevel.Trace);
        }
    }
}
