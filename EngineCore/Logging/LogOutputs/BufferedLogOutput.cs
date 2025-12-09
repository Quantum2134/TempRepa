using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Logging.LogOutputs
{
    public class BufferedLogOutput : ILogOutput
    {
        private readonly List<string> _buffer = new();
        private readonly int _maxLines = 1000;

        public IReadOnlyList<string> Lines => _buffer;

        public void Write(string message, LogLevel level)
        {
            _buffer.Add(message);
            if (_buffer.Count > _maxLines)
                _buffer.RemoveAt(0);
        }

        public void Clear()
        {
            _buffer.Clear();
        }
    }
}
