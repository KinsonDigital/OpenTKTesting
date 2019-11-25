using System;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SDLWithOpenGL.SDL2
{
    public class OpenGLRenderer : IRenderer
    {
        private GameWindow _glWindow;

        public OpenGLRenderer(GameWindow window)
        {
            _glWindow = window;
        }


        public IntPtr WindowHandle { get; set; }

        public bool IsInitialized { get; }


        public void Init(IntPtr glContext)
        {
            //Tell OpenGL that we want textures to be rendered when drawing polygons
            GL.Enable(EnableCap.Texture2D);

            //Next to lines below allow the blending for alpha channels of the image
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }


        public Texture LoadTexture(string path)
        {
            if (!File.Exists($@"Content\{path}"))
                throw new FileNotFoundException($@"File not found at 'Content\{path}");

            //Generate a new texture id
            var id = GL.GenTexture();

            //Any calls after this will be targeted to this texture described by the id
            GL.BindTexture(TextureTarget.Texture2D, id);

            Image<Rgba32> image = (Image<Rgba32>)Image.Load($@"Content\{path}");

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

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels.ToArray());

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return null;
        }


        public void RenderBegin()
        {
            Clear(0,0,0,0);
            GL.Begin(PrimitiveType.Quads);
        }


        public void Render(Texture particle)
        {
            //GL.BindTexture(TextureTarget.Texture2D, _textureId);

            GL.Color4(Color.FromRgba(255, 255, 255, 255).ToColorBytes());
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);

            GL.Color4(Color.FromRgba(255, 255, 255, 255).ToColorBytes());
            GL.TexCoord2(1, 0);
            GL.Vertex2(1, 0);

            GL.Color4(Color.FromRgba(255, 255, 125, 255).ToColorBytes());
            GL.TexCoord2(1, 1);
            GL.Vertex2(1, -1);

            GL.Color4(Color.FromRgba(255, 255, 255, 255).ToColorBytes());
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, -1);
        }


        public void RenderEnd()
        {
            GL.End();
            _glWindow.SwapBuffers();
        }


        public void Clear(byte red, byte green, byte blue, byte alpha)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var pixel = Color.CornflowerBlue.ToPixel<Rgba32>();

            GL.ClearColor(pixel.R, pixel.G, pixel.B, pixel.A);
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
