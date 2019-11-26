using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using NETColor = System.Drawing.Color;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace OpenGLRenderWindow
{
    public class RenderSurfaceWindow : GameWindow
    {
        private List<Texture> _textures = new List<Texture>();


        public RenderSurfaceWindow() : base(640, 480) { }

        public void AddTexture(Texture texture)
        {
            _textures.Add(texture);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //Clear the buffer bit of the color
            var pixel = Color.CornflowerBlue.ToPixel<Rgba32>();
            var cornFlowerBlue = new Color4(pixel.R, pixel.G, pixel.B, pixel.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(cornFlowerBlue);

            GL.Begin(PrimitiveType.Quads);

            var texture = _textures[0];

            GL.BindTexture(TextureTarget.Texture2D, texture.TextureID);

            //TODO: Clean up calc code below and create method for this.  Call method ToGLCartesian()

            GL.Color4(NETColor.White);
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);

            GL.Color4(NETColor.White);
            GL.TexCoord2(1, 0);
            GL.Vertex2(1, 0);

            GL.Color4(NETColor.White);
            GL.TexCoord2(1, 1);
            GL.Vertex2(1, -1);

            GL.Color4(NETColor.White);
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, -1);


            //var centerXPercent = ((float)texture.X + ((float)texture.Width / 2)) / (float)Width;
            //var leftPercent = (float)texture.X / (float)Width;
            //var left = -1f + ((leftPercent - (centerXPercent - leftPercent)) * 2);

            //var centerYPercent = ((float)texture.Y + ((float)texture.Height / 2)) / (float)Height;

            //var topPercent = (float)texture.Y / (float)Height;
            //var top = 1f - ((topPercent - (centerYPercent - topPercent)) * 2);

            //var rightPercent = ((float)texture.X + texture.Width) / (float)Width;
            //var right = -1f + ((rightPercent - (rightPercent - centerXPercent)) * 2);

            //var bottomPercent = ((float)texture.Y + texture.Height) / (float)Height;
            //var bottom = 1f - ((bottomPercent - (bottomPercent - centerYPercent)) * 2);


            //GL.Color4(Color.FromRgba(255, 255, 255, 0).ToColorBytes());
            //GL.TexCoord2(0, 0);
            //GL.Vertex2(left, top);

            //GL.Color4(Color.FromRgba(255, 255, 255, 0).ToColorBytes());
            //GL.TexCoord2(1, 0);
            //GL.Vertex2(right, top);

            //GL.Color4(Color.FromRgba(255, 100, 100, 0).ToColorBytes());
            //GL.TexCoord2(1, 1);
            //GL.Vertex2(right, bottom);

            //GL.Color4(Color.FromRgba(255, 100, 100, 0).ToColorBytes());
            //GL.TexCoord2(0, 1);
            //GL.Vertex2(left, bottom);

            //foreach (var texture in _textures)
            //{
            //    RenderTexture(texture);
            //}

            GL.End();

            SwapBuffers();

            base.OnRenderFrame(e);
        }


        private void RenderTexture(Texture texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture.TextureID);

            //TODO: Clean up calc code below and create method for this.  Call method ToGLCartesian()

            var centerXPercent = ((float)texture.X + ((float)texture.Width / 2)) / (float)Width;
            var leftPercent = (float)texture.X / (float)Width;
            var left = -1f + ((leftPercent - (centerXPercent - leftPercent)) * 2);

            var centerYPercent = ((float)texture.Y + ((float)texture.Height / 2)) / (float)Height;

            var topPercent = (float)texture.Y / (float)Height;
            var top = 1f - ((topPercent - (centerYPercent - topPercent)) * 2);

            var rightPercent = ((float)texture.X + texture.Width) / (float)Width;
            var right = -1f + ((rightPercent - (rightPercent - centerXPercent)) * 2);

            var bottomPercent = ((float)texture.Y + texture.Height) / (float)Height;
            var bottom = 1f - ((topPercent - (topPercent - centerYPercent)) * 2);


            GL.Color4(Color.FromRgba(255, 255, 255, 0).ToColorBytes());
            GL.TexCoord2(0, 0);
            GL.Vertex2(left, top);

            GL.Color4(Color.FromRgba(255, 255, 255, 0).ToColorBytes());
            GL.TexCoord2(1, 0);
            GL.Vertex2(right, top);

            GL.Color4(Color.FromRgba(255, 100, 100, 0).ToColorBytes());
            GL.TexCoord2(1, 1);
            GL.Vertex2(right, bottom);

            GL.Color4(Color.FromRgba(255, 100, 100, 0).ToColorBytes());
            GL.TexCoord2(0, 1);
            GL.Vertex2(left, bottom);
        }
    }
}
