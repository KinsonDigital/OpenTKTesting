using RenderWindowTesting.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenderWindowTesting
{
    public class RenderEngine
    {
        private OpenGLWindow _glWIndow;
        private Task _glWinTask;
        private CancellationTokenSource _glWinTaskTokenSrc;
        private ParticleEngine<Texture> _particleEngine;


        public RenderEngine()
        {
            _particleEngine = new ParticleEngine<Texture>(new TrueRandomizerService());
        }


        public bool IsRunning => _glWIndow.IsRunning;


        public void StartEngine()
        {
            SetupTask();
        }


        public void StopEngine() => throw new NotImplementedException();


        public void Play() => _glWIndow.Play();


        public void Pause() => _glWIndow.Pause();


        public void Restart() => _glWIndow.Restart();


        private void SetupTask()
        {
            _glWinTaskTokenSrc = new CancellationTokenSource();

            _glWinTask = new Task(() =>
            {
                _glWIndow = new OpenGLWindow(new GLInvoker(), _particleEngine);
                _glWIndow.Run(60.0);
            }, _glWinTaskTokenSrc.Token);

            _glWinTask.Start();
        }
    }
}
