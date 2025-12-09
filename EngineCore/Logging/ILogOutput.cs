using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Logging
{
    public interface ILogOutput
    {
        public void Write(string message, LogLevel level);
    }
}
