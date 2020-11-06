using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace OpenTKTesting
{
    public class ImageLoader
    {
        private const string CONTENT_PATH = @"Content\";


        public (int handle, int width, int height) LoadTexture(string name)
        {
            // Generate handle
            var textureHandle = GL.GenTexture();

            var fullPath = $"{CONTENT_PATH}{name}";

            var image = (Image<Rgba32>)Image.Load(fullPath);

            image.Mutate(x => x.Flip(FlipMode.Vertical));

            var tempPixels = new List<Rgba32>();

            for (var i = 0; i < image.Height; i++)
            {
                tempPixels.AddRange(image.GetPixelRowSpan(i).ToArray());
            }

            var pixels = new List<byte>();

            foreach (var pixel in tempPixels)
            {
                pixels.Add(pixel.R);
                pixels.Add(pixel.G);
                pixels.Add(pixel.B);
                pixels.Add(pixel.A);
            }

            var width = image.Width;
            var height = image.Height;

            image.Dispose();

            // Bind the handle
            UseTexture(textureHandle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Load the image
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, tempPixels.ToArray());

            //Unbind the texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return (textureHandle, image.Width, image.Height);
        }


        private void UseTexture(int textureHandle)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);
        }
    }
}
