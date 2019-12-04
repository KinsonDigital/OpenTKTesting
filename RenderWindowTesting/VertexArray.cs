using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace RenderWindowTesting
{
    public class VertexArray : IDisposable
    {
        private int _vertexArrayHandle;


        public VertexArray(VertexBuffer vb, IndexBuffer ib)
        {
            _vertexArrayHandle = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vb.RendererId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib.RendererId);
        }


        public int VertexArrayHandle => _vertexArrayHandle;


        public void Dispose()
        {
            GL.DeleteBuffers(1, ref _vertexArrayHandle);
        }
    }
}
