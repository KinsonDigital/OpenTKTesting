using System;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTesting
{
    //EBO (Element Buffer Object)
    public class IndexBuffer : IDisposable
    {
        private int _rendererId;


        public IndexBuffer(uint[] data)
        {
            Count = data.Length;
            _rendererId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _rendererId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);
        }


        public int Count { get; private set; }

        public int RendererId => _rendererId;


        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, RendererId);
        }


        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }


        public void Dispose()
        {
            Unbind();
            GL.DeleteBuffers(1, ref _rendererId);
        }
    }
}
