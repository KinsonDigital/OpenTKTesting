namespace OpenTKTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            using var window = new Window(500, 500, "LearnOpenTK - Textures");

            window.Run(60.0);
        }
    }
}
