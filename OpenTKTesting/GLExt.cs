using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
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
        private static bool _beginInvoked;
        private static Window _mainWindow;
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

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            return (textureHandle, image.Width, image.Height);
        }


        public static void SetWindow(Window window) => _mainWindow = window;

        public static void RenderTexture(Texture texture)
        {
            GL.BindVertexArray(texture.VAO);
            texture.Use();
            texture.Shaders[0].Use();
            GL.DrawElements(PrimitiveType.Triangles, texture.Indices.Length, DrawElementsType.UnsignedInt, 0);
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


        public static void UseShader(Shader shader) => UseShader(shader.Handle);


        public static void UseShader(int shaderHandle) => GL.UseProgram(shaderHandle);


        public static void Begin()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _beginInvoked = true;
        }


        public static void End()
        {
            if (!_beginInvoked)
                throw new Exception("Begin() must be invoked first.");

            if (_mainWindow is null)
                throw new Exception("The Window has not been set.");

            _mainWindow.SwapBuffers();

            _beginInvoked = false;
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
