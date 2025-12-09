namespace EngineEditor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (RuntimeGame game = new RuntimeGame("Runtime game", 1080, 720))
            {
                game.Run();
            }
        }
    }
}
