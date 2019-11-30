using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTesting
{
    public class VertexArray
    {
        public VertexArray(VertexBuffer vb, IndexBuffer ib)
        {
            VertexArrayHandle = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vb.RendererId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib.RendererId);
        }


        public int VertexArrayHandle { get; private set; }
    }
}
