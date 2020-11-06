using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTKTesting
{
    public class Window : GameWindow
    {
        #region Private Fields
        private Texture _texture;
        private float _elapsedMS;
        private readonly Renderer _renderer;
        private static bool _beginInvoked;
        private bool _increaseSize = true;
        private bool _increaseAlpha;
        #endregion


        #region Props
        public static int ViewPortWidth { get; private set; }

        public static int ViewPortHeight { get; private set; }
        #endregion


        #region Constructors
        public Window(GameWindowSettings gameWinSettings, NativeWindowSettings nativeWinSettings)
            : base(gameWinSettings, nativeWinSettings)
        {
            ViewPortWidth = nativeWinSettings.Size.X;
            ViewPortHeight = nativeWinSettings.Size.Y;

            _renderer = new Renderer();
        }
        #endregion


        #region Protected Methods
        protected override void OnLoad()
        {
            GLExt.EnableAlpha();

            GLExt.ClearColor(100, 148, 237, 255);

            _texture = new Texture("Gear.png");
            _texture.X = 300;
            _texture.Y = 300;

            base.OnLoad();
        }


        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            _elapsedMS += (float)args.Time * 1000f;

            if (_elapsedMS >= 16f && false)
            {
                _texture.X += 25f * (float)args.Time;
                _texture.Y += 25f * (float)args.Time;
                _texture.Angle += 1;

                _texture.Size += _increaseSize ? 0.015f : -0.015f;

                if (_texture.Size >= 2)
                {
                    _increaseSize = false;
                }
                else if(_texture.Size <= 1)
                {
                    _increaseSize = true;
                }

                _elapsedMS = 0;

                RandomColor();
            }

            base.OnUpdateFrame(args);
        }

        private void RandomColor()
        {
            if (_texture.TintColor.A >= 255)
            {
                _increaseAlpha = false;
            }
            else if (_texture.TintColor.A <= 0)
            {
                _increaseAlpha = true;
            }

            _texture.TintColor = Color.FromArgb(_increaseAlpha ? _texture.TintColor.A + 1 : _texture.TintColor.A - 1, 255, 0, 255);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Begin();

            _renderer.Draw(_texture);

            End();

            base.OnRenderFrame(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, this.Size.X, this.Size.Y);
            
            ViewPortWidth = this.Size.X;
            ViewPortHeight = this.Size.Y;

            _texture.Update();

            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            _texture.Dispose();

            GL.UseProgram(0);

            base.OnUnload();
        }
        #endregion


        #region Private Methods
        private void Begin()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _beginInvoked = true;
        }


        private void End()
        {
            if (!_beginInvoked)
                throw new Exception("Begin() must be invoked first.");

            SwapBuffers();

            _beginInvoked = false;
        }
        #endregion
    }
}
