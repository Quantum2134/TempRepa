using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Logging.LogOutputs
{
    public class ConsoleLogOutput : ILogOutput
    {
        public void Write(string message, LogLevel level)
        {
            switch (level)
            {              
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }

            Console.WriteLine(message);

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
