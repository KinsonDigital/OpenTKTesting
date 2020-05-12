using OpenTK;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using NETColor = System.Drawing.Color;

namespace OpenTKTesting
{
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture : IDisposable
    {
        private readonly ImageLoader _imageLoader;
        private float _x;
        private float _y;
        private NETColor _tintColor = NETColor.White;


        #region Props
        public float X
        {
            get => _x;
            set
            {
                _x = value;
                Update();
            }
        }

        public float Y
        {
            get => _y;
            set
            {
                _y = value;
                Update();
            }
        }

        public float Angle { get; set; }

        public float Size { get; set; } = 1f;

        public int Width { get; set; }

        public int Height { get; set; }

        public NETColor TintColor
        {
            get => _tintColor;
            set
            {
                _tintColor = value;

                var red = GLExt.MapValue(0, 255, 0, 1, _tintColor.R);
                var green = GLExt.MapValue(0, 255, 0, 1, _tintColor.G);
                var blue = GLExt.MapValue(0, 255, 0, 1, _tintColor.B);
                var alpha = GLExt.MapValue(0, 255, 0, 1, _tintColor.A);

                //Use this for vec3 uniform data
                GLExt.SetVec4Uniform(Shaders[0].ProgramHandle, "u_tintClr", new Vector4(red, green, blue, alpha));
            }
        }

        internal int Handle { get; set; }

        internal List<Shader> Shaders { get; set; } = new List<Shader>();

        internal VertexBuffer VB { get; private set; }

        internal IndexBuffer IB { get; private set; }

        internal VertexArray VA { get; private set; }

        internal float[] Vertices { get; set; }

        internal uint[] Indices { get; set; }
        #endregion


        #region Constructors
        // Create texture from path.
        public Texture(string name)
        {
            Indices = new uint[]
            {
                0, 1, 3,
                1, 2, 3
            };
                    
            _imageLoader = new ImageLoader();
            var (handle, width, height) = _imageLoader.LoadTexture(name);

            Handle = handle;
            Width = width;
            Height = height;

            Vertices = new[] {
                //Position              Texture coordinates
               -1f,   1f, 0.0f,     0.0f, 1.0f, // top left 
                1f,   1f, 0.0f,     1.0f, 1.0f, // top right
                1f,  -1f, 0.0f,     1.0f, 0.0f, // bottom right
               -1f,  -1f, 0.0f,     0.0f, 0.0f  // bottom left
            };

            VB = new VertexBuffer(Vertices);
            IB = new IndexBuffer(Indices);
            VA = new VertexArray(VB, IB);

            CreateDefaultShader();

            TintColor = NETColor.Red;
        }
        #endregion


        #region Pubic Methods
        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Use()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }


        public void Update()
        {
            UpdateTransformData(BuildTransformationMatrix());
        }
        #endregion


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

            UpdateTransformData(BuildTransformationMatrix());
        }


        public void Dispose()
        {
            VB.Dispose();
            IB.Dispose();
            VA.Dispose();

            GL.DeleteTexture(Handle);

            Shaders.ForEach(s => s.Dispose());
        }


        private Matrix4 BuildTransformationMatrix()
        {
            var scaleX = (float)Width / Window.ViewPortWidth;
            var scaleY = (float)Height / Window.ViewPortHeight;

            scaleX *= Size;
            scaleY *= Size;

            //NDC = Normalized Device Coordinates
            var ndcX = GLExt.MapValue(0, Window.ViewPortWidth, -1f, 1f, _x);
            var ndcY = GLExt.MapValue(0, Window.ViewPortHeight, 1f, -1f, _y);

            //NOTE: (+ degrees) rotates CCW and (- degress) rotates CW
            var angleRadians = MathHelper.DegreesToRadians(Angle);

            //Invert angle to rotate CW instead of CCW
            angleRadians *= -1;

            var rotation = Matrix4.CreateRotationZ(angleRadians);
            var scale = Matrix4.CreateScale(scaleX, scaleY, 1f);
            var posMatrix = Matrix4.CreateTranslation(new Vector3(ndcX, ndcY, 0));


            return rotation * scale * posMatrix;
        }


        private void UpdateTransformData(Matrix4 transMatrix)
        {
            GLExt.SetMat4Uniform(Shaders[0].ProgramHandle, "transform", transMatrix);
        }
        #endregion
    }
}
