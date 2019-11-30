using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTKTesting
{
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture : IDisposable
    {
        #region Props
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        internal int Handle { get; set; }

        internal List<Shader> Shaders { get; set; } = new List<Shader>();

        internal VertexBuffer VB { get; private set; }

        internal IndexBuffer IB { get; private set; }

        internal VertexArray VA { get; private set; }

        internal float[] Vertices { get; set; }

        internal uint[] Indices { get; set; }
        #endregion


        // Create texture from path.
        public Texture(string name)
        {
            //Square - Position & Texture coordinates/vertices all in one array
            //Vertices = new [] {
            //  //Position                Texture coordinates
            //    -0.5f, 0.5f, 0.0f,      0.0f, 1.0f,  // top left 
            //    0.5f, 0.5f, 0.0f,       1.0f, 1.0f, // top right
            //    0.5f, -0.5f, 0.0f,      1.0f, 0.0f, // bottom right
            //    -0.5f, -0.5f, 0.0f,     0.0f, 0.0f // bottom left
            //};
            
            Indices = new uint[]
            {
                0, 1, 3,
                1, 2, 3
            };

            var (handle, width, height) = GLExt.LoadTexture(name);

            Handle = handle;
            Width = width;
            Height = height;

            Vertices = MapToNDC();


            VB = new VertexBuffer(Vertices);
            IB = new IndexBuffer(Indices);
            VA = new VertexArray(VB, IB);

            CreateDefaultShader();
        }


        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Use() => GLExt.UseTexture(Handle);


        #region Private Methods
        private float[] MapToNDC()
        {
            var vertices = new [] {
                //Position              Texture coordinates
                0.0f,   0.0f, 0.0f,     0.0f, 1.0f, // top left 
                0.0f,   0.0f, 0.0f,     1.0f, 1.0f, // top right
                0.0f,   0.0f, 0.0f,     1.0f, 0.0f, // bottom right
                0.0f,   0.0f, 0.0f,     0.0f, 0.0f  // bottom left
            };

            var posX = GLExt.MapValue(0, Window.ViewPortWidth, -1, 1, X);
            var posY = GLExt.MapValue(0, Window.ViewPortHeight, 1, -1, Y);
            var newWidth = GLExt.MapValue(0, Window.ViewPortWidth, -1, 1, X + Width);
            var newHeight = GLExt.MapValue(0, Window.ViewPortHeight, 1, -1, Y + Height);

            vertices[0] = posX;
            vertices[1] = posY;
            vertices[5] = newWidth;
            vertices[6] = posY;
            vertices[10] = newWidth;
            vertices[11] = newHeight;
            vertices[15] = posX;
            vertices[16] = newHeight;


            return vertices;
        }


        private void CreateDefaultShader()
        {
            // The shaders have been modified to include the texture coordinates, check them out after finishing the OnLoad function.
            var shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();

            // Because there is 5 floats between the start of the first vertex and the start of the second,
            // we set this to 5 * sizeof(float).
            // This will now pass the new vertex array to the buffer.
            GLExt.SetupVertexShaderAttribute(shader, "aPosition", 3, 0);

            // Next, we also setup texture coordinates. It works in much the same way.
            // We add an offset of 3, since the first vertex coordinate comes after the first vertex
            // and change the amount of data to 2 because there's only 2 floats for vertex coordinates
            GLExt.SetupVertexShaderAttribute(shader, "aTexCoord", 2, 3 * sizeof(float));

            Shaders.Add(shader);
        }

        public void Dispose()
        {
            VB.Dispose();
            IB.Dispose();

            GL.DeleteTexture(Handle);

            foreach (var shader in Shaders)
            {
                GL.DeleteProgram(shader.Handle);
            }
        }
        #endregion
    }
}
