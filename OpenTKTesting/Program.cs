using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace OpenTKTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameWindowSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(500, 500),
            };

            using var window = new Window(gameWindowSettings, nativeWindowSettings);
            window.Run();
        }
    }
}
