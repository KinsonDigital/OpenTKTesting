using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTKTesting
{
    public class Window : GameWindow
    {
        // Because we're adding a texture, we modify the vertex array to include texture coordinates.
        // Texture coordinates range from 0.0 to 1.0, with (0.0, 0.0) representing the bottom left, and (1.0, 1.0) representing the top right
        // The new layout is three floats to create a vertex, then two floats to create the coordinates
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left 
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _elementBufferObject;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;

        // For documentation on this, check Texture.cs
        private Texture _texture;
        private KeyboardState _currentKeyState;
        private KeyboardState _prevKeyState;


        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            GLExt.SetWindow(this);
        }


        protected override void OnLoad(EventArgs e)
        {
            GLExt.EnableAlpha();

            GLExt.ClearColor(255, 0, 0, 255);

            _texture = new Texture("Gear.png");
            
            base.OnLoad(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GLExt.Begin();

            GLExt.RenderTexture(_texture);

            GLExt.End();

            base.OnRenderFrame(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _currentKeyState = Keyboard.GetState();

            if (_currentKeyState.IsKeyDown(Key.Escape))
                Exit();

            if (_currentKeyState.IsKeyDown(Key.A) && _prevKeyState.IsKeyUp(Key.A))
            {
                if (GLExt.IsAlphaEnabled)
                    GLExt.DisableAlpha();
                else
                    GLExt.EnableAlpha();
            }

            _prevKeyState = _currentKeyState;
            base.OnUpdateFrame(e);
        }


        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }


        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);
            // Don't forget to dispose of the texture too!
            GL.DeleteTexture(_texture.Handle);
            base.OnUnload(e);
        }
    }

}
