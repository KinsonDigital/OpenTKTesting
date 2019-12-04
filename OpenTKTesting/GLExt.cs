using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace OpenTKTesting
{
    public static class GLExt
    {
        #region Fields
        private static string _contentPath = @"Content\";
        #endregion


        #region Props
        public static bool IsAlphaEnabled { get; private set; } = false;
        #endregion


        #region Public Methods
        public static (int handle, int width, int height) LoadTexture(string name)
        {
            // Generate handle
            var textureHandle = GL.GenTexture();

            // Bind the handle
            UseTexture(textureHandle);

            var fullPath = $"{_contentPath}{name}";

            var image = (Image<Rgba32>)Image.Load($@"Content\{name}");

            image.Mutate(x => x.Flip(FlipMode.Vertical));

            Rgba32[] tempPixels = image.GetPixelSpan().ToArray();

            var pixels = new List<byte>();

            foreach (var pixel in tempPixels)
            {
                pixels.Add(pixel.R);
                pixels.Add(pixel.G);
                pixels.Add(pixel.B);
                pixels.Add(pixel.A);
            }

            // Load the image
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tempPixels.ToArray());

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            return (textureHandle, image.Width, image.Height);
        }


        public static void RenderTexture(Texture texture)
        {
            GL.BindVertexArray(texture.VA.VertexArrayHandle);
            texture.Use();
            texture.Shaders[0].Use();
            GL.DrawElements(PrimitiveType.Triangles, texture.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }


        public static void SetMat4Uniform(int shaderProgramHandle, string uniformName, Matrix4 matrix)
        {
            var uniformTransformationLocation = GL.GetUniformLocation(shaderProgramHandle, uniformName);
            GL.UniformMatrix4(uniformTransformationLocation, true, ref matrix);
        }


        public static void SetVec3Uniform(int shaderProgramHandle, string uniformName, Vector3 vector)
        {
            //TODO: Make use of the cached uniform locations in the Shader class.
            var vec3Location = GL.GetUniformLocation(shaderProgramHandle, uniformName);

            if (vec3Location == -1)
                throw new Exception($"The uniform with the name '{uniformName}' does not exist.");

            GL.Uniform3(vec3Location, ref vector);
        }


        public static void SetVec4Uniform(int shaderProgramHandle, string uniformName, Vector4 vector)
        {
            //TODO: Make use of the cached uniform locations in the Shader class.
            var vec4Location = GL.GetUniformLocation(shaderProgramHandle, uniformName);

            if (vec4Location == -1)
                throw new Exception($"The uniform with the name '{uniformName}' does not exist.");

            GL.Uniform4(vec4Location, ref vector);
        }


        public static void EnableAlpha()
        {
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            IsAlphaEnabled = true;
        }


        public static void DisableAlpha()
        {
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            IsAlphaEnabled = false;
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
        }


        public static void ClearColor(byte r, byte g, byte b, byte a)
        {
            var resultR = MapValue(0f, 255f, 0f, 1f, r);
            var resultG = MapValue(0f, 255f, 0f, 1f, g);
            var resultB = MapValue(0f, 255f, 0f, 1f, b);
            var resultA = MapValue(0f, 255f, 0f, 1f, a);

            GL.ClearColor(resultR, resultG, resultB, resultA);
        }


        public static int CreateVBO(float[] vertices)
        {
            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);


            return vertexBufferObject;
        }


        public static int CreateEBO(uint[] indices)
        {
            var elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);


            return elementBufferObject;
        }


        public static int CreateVAO(int vbo, int ebo)
        {
            var vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);


            return vertexArrayObject;
        }


        public static void SetupVertexShaderAttribute(Shader shader, string attrName, int size, int offSet)
        {
            var vertexLocation = shader.GetAttribLocation(attrName);
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, size, VertexAttribPointerType.Float, false, 5 * sizeof(float), offSet);
        }


        public static void UseTexture(Texture texture) => UseTexture(texture.Handle);


        public static void UseTexture(int textureHandle)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);
        }


        public static void UseShader(Shader shader) => UseShader(shader.ProgramHandle);


        public static void UseShader(int shaderHandle) => GL.UseProgram(shaderHandle);


        public static int CreateShader(ShaderType shaderType, string shaderSrc)
        {
            var fragmentShader = GL.CreateShader(shaderType);
            GL.ShaderSource(fragmentShader, shaderSrc);
            CompileShader(fragmentShader);


            return fragmentShader;
        }


        public static void CompileShader(int shaderHandle)
        {
            // Try to compile the shader
            GL.CompileShader(shaderHandle);

            // Check for compilation errors
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out var code);

            if (code != (int)All.True)
            {
                var errorInfo = GL.GetShaderInfoLog(shaderHandle);
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                throw new Exception($"Error occurred while compiling Shader({shaderHandle})\n{errorInfo}");
            }
        }


        public static void DestroyShader(int program, int shaderHandle)
        {
            GL.DetachShader(program, shaderHandle);
            GL.DeleteShader(shaderHandle);
        }


        public static int CreateShaderProgram(int vertexShaderHandle, int fragmentShaderHandle)
        {
            var programHandle = GL.CreateProgram();

            //Attach both shaders...
            GL.AttachShader(programHandle, vertexShaderHandle);
            GL.AttachShader(programHandle, fragmentShaderHandle);

            //Link them together
            LinkProgram(programHandle);


            return programHandle;
        }


        private static void LinkProgram(int programHHandle)
        {
            // We link the program
            GL.LinkProgram(programHHandle);

            // Check for linking errors
            GL.GetProgram(programHHandle, GetProgramParameterName.LinkStatus, out var code);

            if (code != (int)All.True)
            {
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred whilst linking Program({programHHandle})");
            }
        }
        #endregion


        #region Private Methods
        public static float MapValue(float from0, float from1, float to0, float to1, float value)
        {
            //http://james-ramsden.com/map-a-value-from-one-number-scale-to-another-formula-and-c-code/
            return to0 + (to1 - to0) * ((value - from0) / (from1 - from0));
        }
        #endregion
    }
}
