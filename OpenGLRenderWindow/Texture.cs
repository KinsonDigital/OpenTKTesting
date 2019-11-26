using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGLRenderWindow
{
    public class Texture
    {
        public Texture(int textureId, int width, int height)
        {
            TextureID = textureId;
            Width = width;
            Height = height;
        }


        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int TextureID { get; set; }
    }
}
