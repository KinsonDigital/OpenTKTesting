using OpenTK;
using OpenTK.Graphics.ES20;
using System;

namespace OpenTKTesting
{
    class Program
    {
        private static Game _game;

        static void Main(string[] args)
        {
            _game = new Game(640, 480);

            _game.Run();
        }
    }
}
