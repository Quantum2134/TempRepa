using EngineCore.Logging.Profiling;
using System;

namespace EngineEditor
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (Editor editor = new Editor("Quantum Editor", 1920, 1080))
            {
                editor.Run();
            }
        }
    }
}
