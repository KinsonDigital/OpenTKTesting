using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTKSample
{
    public class Game : GameWindow
    {
        private int _vertexBufferObj;//Handle to a VBO.  This is a handle to the buffered vertices in the vertex shader
        private int _vertexArrayObj;//Handle to a VAO.  This is a handle to the VBO setup for ease of VBO usage


        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }


        /// <summary>
        /// Vertices for a triangle
        /// </summary>
        public float[] Vertices => new float[]
        {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, -0.0f,//Bottom-right vertex
            0.0f, 0.5f, 0.0f//Top Vertex
        };


        public Shader Shader { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexBufferObj = GL.GenBuffer();

            //1. Copy our vertices array in a buffer for OpenGL to use
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArrayObj);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            Shader = new Shader("shader.vert", "shader.frag");

            Shader.Use();

            //2. bind Vertext Array Object (VAO)
            GL.BindVertexArray(_vertexBufferObj);


            //3. Set our vertex attributes pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //4. Bind the VBO again.
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArrayObj);

            base.OnLoad(e);
        }


        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            GL.DeleteBuffer(_vertexBufferObj);
            GL.DeleteVertexArray(_vertexArrayObj);

            GL.DeleteProgram(Shader.Program);

            Shader.Dispose();

            base.OnUnload(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
                Exit();

            base.OnUpdateFrame(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);//Always call this first

            Shader.Use();

            GL.BindVertexArray(_vertexArrayObj);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }
    }
}
