using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Platform;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using WPFTestingUI.OpenGL;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace WPFTestingUI
{
    public class RenderEngine
    {
        private Task _glWinTask;
        private CancellationTokenSource _glWinTaskTokenSrc;
        private Texture _texture;
        private IRenderer _renderer;

        public RenderEngine(Renderer renderer)
        {
            _renderer = renderer;

            _texture = new Texture("Link.png");
        }


        public bool IsRunning { get; set; }


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

                _renderer.Render(_texture);
            }
        }
    }
}
