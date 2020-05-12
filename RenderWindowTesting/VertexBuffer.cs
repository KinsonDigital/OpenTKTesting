using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace RenderWindowTesting
{
    //VBO (Vertext Buffer Object)
    public class VertexBuffer : IDisposable
    {
        private int _rendererId;


        public VertexBuffer(float[] data)
        {
            _rendererId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererId);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
        }


        public int RendererId => _rendererId;


        public void SetLayout(float[] data)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
        }


        public void Bind() 
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, RendererId);
        }


        public void Unbind() 
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        public void Dispose()
        {
            Unbind();

            GL.DeleteBuffers(1, ref _rendererId);
        }
    }
}
