using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WPFTestingUI.OpenGL;

namespace WPFTestingUI
{
    public class RenderEngine
    {
        private Task _glWinTask;
        private CancellationTokenSource _glWinTaskTokenSrc;
        private List<Texture> _textures = new List<Texture>();
        private IRenderer _renderer;


        public RenderEngine(IRenderer renderer)
        {
            _renderer = renderer;
        }


        public bool IsRunning { get; set; }


        #region Public Methods
        public void StartEngine()
        {
            SetupTask();
        }


        public void StopEngine() => throw new NotImplementedException();


        public void Play() => IsRunning = true;


        public void Pause() => IsRunning = false;

        
        public void Reload()
        {
            //Pause();

            //_texture.Dispose();
            //_texture = null;

            ////_texture = new Texture("Link.png");

            //Play();
        }


        public void AddTexture(Texture texture)
        {
            _textures.Add(texture);
        }
        #endregion


        #region Private Methods
        private void SetupTask()
        {
            _glWinTaskTokenSrc = new CancellationTokenSource();

            _glWinTask = new Task(Loop, _glWinTaskTokenSrc.Token);

            _glWinTask.Start();
        }


        private void Loop()
        {
            while(!_glWinTaskTokenSrc.IsCancellationRequested)
            {
                _glWinTask.Wait(60);

                _renderer.Render(_textures.ToArray());
                //_textures.ForEach(t => _renderer.Render(t));
            }
        }
        #endregion
    }
}
