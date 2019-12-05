using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using NETColor = System.Drawing.Color;

namespace RenderWindowTesting
{
    public class Texture : IDisposable
    {
        private IGLInvoker _gl;
        private readonly ImageLoader _imageLoader;
        private float _x;
        private float _y;
        private NETColor _tintColor = NETColor.FromArgb(0, 255, 255, 255);


        #region Props
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

                var red = _tintColor.R.MapValue(0, 255, 0, 1);
                var green = _tintColor.G.MapValue(0, 255, 0, 1);
                var blue = _tintColor.B.MapValue(0, 255, 0, 1);
                var alpha = _tintColor.A.MapValue(0, 255, 0, 1);

                //Use this for vec3 uniform data
                _gl.SetVec4Uniform(Shaders[0].ProgramHandle, "u_tintClr", new Vector4(red, green, blue, alpha));
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
            _gl = new GLInvoker();

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
            var shader = new Shader(_gl, "Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();

            // Because there is 5 floats between the start of the first vertex and the start of the second,
            // we set this to 5 * sizeof(float).
            // This will now pass the new vertex array to the buffer.
            _gl.SetupVertexShaderAttribute(shader, "aPosition", 3, 0);

            // Next, we also setup texture coordinates. It works in much the same way.
            // We add an offset of 3, since the first vertex coordinate comes after the first vertex
            // and change the amount of data to 2 because there's only 2 floats for vertex coordinates
            _gl.SetupVertexShaderAttribute(shader, "aTexCoord", 2, 3 * sizeof(float));

            Shaders.Add(shader);

            UpdateTransformData(BuildTransformationMatrix());
        }


        public void Dispose()
        {
            VB.Dispose();
            IB.Dispose();
            VA.Dispose();

            GL.DeleteTexture(Handle);

            foreach (var shader in Shaders)
            {
                GL.DeleteProgram(shader.ProgramHandle);
            }
        }


        private Matrix4 BuildTransformationMatrix()
        {
            var scaleX = (float)Width / OpenGLWindow.ViewPortWidth;
            var scaleY = (float)Height / OpenGLWindow.ViewPortHeight;

            scaleX *= Size;
            scaleY *= Size;

            //NDC = Normalized Device Coordinates
            var ndcX = _x.MapValue(0, OpenGLWindow.ViewPortWidth, -1f, 1f);
            var ndcY = _y.MapValue(0, OpenGLWindow.ViewPortHeight, 1f, -1f);

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
            _gl.SetMat4Uniform(Shaders[0].ProgramHandle, "transform", transMatrix);
        }
        #endregion
    }
}
