using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Logging
{
    public static class Logger
    {      
        private static readonly List<ILogOutput> logOutputs = new();

        public static event Action<string, LogLevel>? OnLogMessage;

        public static void AddLogOutput(ILogOutput sink)
        {
            logOutputs.Add(sink);
        }

        public static bool RemoveLogOutput(ILogOutput sink)
        {
            return logOutputs.Remove(sink);
        }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            var fullMessage = $"[{level}]: {message}";
            foreach (var sink in logOutputs)
                sink.Write(fullMessage, level);

            OnLogMessage?.Invoke(fullMessage, level);
        }
    }
}
