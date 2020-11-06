using OpenTK.Graphics.OpenGL4;

namespace OpenTKTesting
{
    public class Renderer
    {
        public void Draw(Texture texture)
        {
            GL.BindVertexArray(texture.VA.VertexArrayHandle);
            texture.Use();
            texture.Shaders[0].Use();
            GL.DrawElements(PrimitiveType.Triangles, texture.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
