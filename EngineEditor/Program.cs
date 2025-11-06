using EngineCore.Logging.Profiling;

namespace EngineEditor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Editor editor = new Editor("Quantum Editor", 1920, 1080))
            {
                editor.Run();
            }
        }
    }
}
