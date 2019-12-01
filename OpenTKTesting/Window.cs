using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace OpenTKTesting
{
    public class Window : GameWindow
    {
        private Texture _texture;
        private KeyboardState _currentKeyState;
        private KeyboardState _prevKeyState;
        private float _elapsedMS;
        private readonly Renderer _renderer;
        private static bool _beginInvoked;

        public static int ViewPortWidth { get; private set; }

        public static int ViewPortHeight { get; private set; }


        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            ViewPortWidth = width;
            ViewPortHeight = height;
            _renderer = new Renderer();
        }


        protected override void OnLoad(EventArgs e)
        {
            GLExt.EnableAlpha();

            GLExt.ClearColor(255, 0, 0, 255);

            _texture = new Texture("Gear.png");
            
            base.OnLoad(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _currentKeyState = Keyboard.GetState();

            _elapsedMS += (float)e.Time * 1000f;

            if (_elapsedMS >= 16f)
            {
                _texture.X += 1;
                _texture.Y += 1;
                _elapsedMS = 0;
            }

            if (_currentKeyState.IsKeyDown(Key.Escape))
                Exit();

            _prevKeyState = _currentKeyState;

            base.OnUpdateFrame(e);
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
            base.OnResize(e);
        }


        protected override void OnUnload(EventArgs e)
        {
            _texture.Dispose();

            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindVertexArray(0);
            
            //GL.DeleteVertexArray(_vertexArrayObject);

            GL.UseProgram(0);
            

            base.OnUnload(e);
        }



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
    }
}
