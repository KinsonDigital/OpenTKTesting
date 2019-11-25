using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using NETPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace OpenTKTesting
{
    public class TextureLoader
    {
        public static int Load(string path)
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

            return id;
        }
    }
}
