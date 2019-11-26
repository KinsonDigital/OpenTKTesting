using System;

namespace OpenGLRenderWindow
{
    public class Program
    {
        public static Texture _texture;
        public static RenderSurfaceWindow _renderWindow;

        static void Main(string[] args)
        {
            _renderWindow = new RenderSurfaceWindow();

            _texture = TextureLoader.Load($@"gear.png");
            _texture.X = 100;
            _texture.Y = 100;

            _renderWindow.AddTexture(_texture);

            _renderWindow.Run();
        }
    }
}
