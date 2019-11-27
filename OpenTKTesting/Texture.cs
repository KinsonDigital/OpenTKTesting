using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Linq;

namespace OpenTKTesting
{
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture
    {
        #region Fields
        #endregion


        #region Props
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        internal int Handle { get; set; }

        internal List<Shader> Shaders { get; set; } = new List<Shader>();

        internal int VBO { get; private set; }

        internal int EBO { get; private set; }

        internal int VAO { get; private set; }

        internal float[] Vertices { get; set; }

        internal uint[] Indices { get; set; }
        #endregion


        // Create texture from path.
        public Texture(string name)
        {
            //Square - Position & Texture coordinates/vertices all in one array
            Vertices = new [] {
              //Position                Texture coordinates
                0.5f, 0.5f, 0.0f,       1.0f, 1.0f, // top right
                0.5f, -0.5f, 0.0f,      1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f,     0.0f, 0.0f, // bottom left
                -0.5f, 0.5f, 0.0f,      0.0f, 1.0f  // top left 
            };
            
            Indices = new uint[]
            {
                0, 1, 3,
                1, 2, 3
            };

            var (handle, width, height) = GLExt.LoadTexture(name);

            Handle = handle;
            Width = width;
            Height = height;

            var widthDeltaPercent = Width / Window.ViewPortWidth;
            var heightDeltaPercent = Height / Window.ViewPortHeight;

            //Calculate screen space
            var topRightX = GLExt.MapValue(-1, 1, 0, Width, Vertices[0]);
            var topRightY = GLExt.MapValue(1, -1, 0, Height, Vertices[1]);

            var bottomRightX = GLExt.MapValue(-1, 1, 0, Width, Vertices[5]);
            var bottomRightY = GLExt.MapValue(1, -1,0 , Height, Vertices[6]);

            var bottomLeftX = GLExt.MapValue(-1, 1, 0, Width, Vertices[10]);
            var bottomLeftY = GLExt.MapValue(1, -1, 0, Height, Vertices[11]);

            var topLeftX = GLExt.MapValue(-1, 1, 0, Width, Vertices[15]);
            var topLeftY = GLExt.MapValue(1, -1, 0, Height, Vertices[16]);

            //var topRightX = GLExt.MapValue(0, Width, -1, 1, Vertices[0]);
            //var topRightY = GLExt.MapValue(0, Height, 1, -1, Vertices[1]);

            //var bottomRightX = GLExt.MapValue(0, Width, -1, 1, Vertices[5]);
            //var bottomRightY = GLExt.MapValue(0, Height, 1, -1, Vertices[6]);

            //var bottomLeftX = GLExt.MapValue(0, Width, -1, 1, Vertices[10]);
            //var bottomLeftY = GLExt.MapValue(0, Height, 1, -1, Vertices[11]);

            //var topLeftX = GLExt.MapValue(0, Width, -1, 1, Vertices[15]);
            //var topLeftY = GLExt.MapValue(0, Height, 1, -1, Vertices[16]);

            VBO = GLExt.CreateVBO(Vertices);
            EBO = GLExt.CreateEBO(Indices);
            VAO = GLExt.CreateVAO(VBO, EBO);

            CreateDefaultShader();
        }


        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Use() => GLExt.UseTexture(Handle);


        #region Private Methods
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
        #endregion
    }
}
