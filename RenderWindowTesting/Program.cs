using RenderWindowTesting.Services;
using System.Drawing;

namespace RenderWindowTesting
{
    class Program
    {
        private static IGLInvoker _gl;
        private static IRandomizerService _randomService;
        private static ParticleEngine<Texture> _particleEngine;


        static void Main(string[] args)
        {
            var renderEngine = new RenderEngine();

            renderEngine.StartEngine();

            return;

            _gl = new GLInvoker();
            _randomService = new TrueRandomizerService();

            var setup = new ParticleSetup()
            {
                UseColorsFromList = true,
                Colors = new Color[]
                {
                    Color.FromArgb(255, 255, 0, 0),
                    Color.FromArgb(255, 0, 255, 0),
                    Color.FromArgb(255, 0, 0, 255)
                }
            };

            _particleEngine = new ParticleEngine<Texture>(_randomService)
            {
                SpawnLocation = new PointF(300, 300)
            };
            _particleEngine.ApplySetup(setup);


            using var window = new OpenGLWindow(_gl, _particleEngine);

            window.Run(60.0);
        }
    }
}
