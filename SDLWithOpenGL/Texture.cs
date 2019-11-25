using SDLWithOpenGL.SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SDLWithOpenGL
{
    public class Texture
    {
        private readonly ISDLInvoker _sdlInvoker;


        public Texture(ISDLInvoker sdlInvoker, IntPtr texturePtr, int width, int height)
        {
            _sdlInvoker = sdlInvoker;
            TexturePointer = new SDLSafeHandle(texturePtr);
            Width = width;
            Height = height;
        }


        public string Name { get; set; }

        public PointF Position { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Color TintColor { get; set; }

        public float Size { get; set; } = 1.0f;

        public SDLSafeHandle TexturePointer { get; set; }

        public float Angle { get; set; }
    }
}
