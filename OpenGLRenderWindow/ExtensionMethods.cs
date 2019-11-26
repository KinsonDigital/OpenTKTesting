using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGLRenderWindow
{
    public static class ExtensionMethods
    {
        public static byte[] ToColorBytes(this Color color)
        {
            var pixel = color.ToPixel<Rgba32>();


            return new byte[] { pixel.R, pixel.G, pixel.B, pixel.A };
        }
    }
}
