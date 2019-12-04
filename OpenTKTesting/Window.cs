using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace OpenTKTesting
{
    public class Window : GameWindow
    {
        #region Private Fields
        private Texture _texture;
        private KeyboardState _currentKeyState;
        private KeyboardState _prevKeyState;
        private float _elapsedMS;
        private readonly Renderer _renderer;
        private static bool _beginInvoked;
        private bool _increaseSize = true;
        private bool _increaseRed;
        #endregion


        #region Props
        public static int ViewPortWidth { get; private set; }

        public static int ViewPortHeight { get; private set; }
        #endregion


        #region Constructors
        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            ViewPortWidth = width;
            ViewPortHeight = height;
            _renderer = new Renderer();
        }
        #endregion


        #region Protected Methods
        protected override void OnLoad(EventArgs e)
        {
            GLExt.EnableAlpha();

            GLExt.ClearColor(100, 148, 237, 255);

            _texture = new Texture("Gear.png");
            
            base.OnLoad(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _currentKeyState = Keyboard.GetState();

            _elapsedMS += (float)e.Time * 1000f;

            if (_elapsedMS >= 16f)
            {
                _texture.X += 25f * (float)e.Time;
                _texture.Y += 25f * (float)e.Time;
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

            if (_currentKeyState.IsKeyDown(Key.Escape))
                Exit();

            _prevKeyState = _currentKeyState;

            base.OnUpdateFrame(e);
        }

        private void RandomColor()
        {
            if (_texture.TintColor.R >= 255)
            {
                _increaseRed = false;
            }
            else if (_texture.TintColor.R <= 0)
            {
                _increaseRed = true;
            }

            _texture.TintColor = Color.FromArgb(255, _increaseRed ? _texture.TintColor.R + 1 : _texture.TintColor.R - 1, 0, 0);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Begin();

            _renderer.Draw(_texture);

            End();

            base.OnRenderFrame(e);
        }


        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            ViewPortWidth = Width;
            ViewPortHeight = Height;

            _texture.Update();

            base.OnResize(e);
        }


        protected override void OnUnload(EventArgs e)
        {
            _texture.Dispose();

            GL.UseProgram(0);

            base.OnUnload(e);
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
