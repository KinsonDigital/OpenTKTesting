using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WPFTestingUI.OpenGL;

namespace WPFTestingUI
{
    public class Renderer : IRenderer
    {
        private readonly GLControl _glControl;
        private readonly IGLInvoker _gl;
        private bool _increaseSize;
        private bool _increaseAngle;
        private float _angle;
        private Texture _currentTexture;
        private Texture[] _textures;


        public IntPtr WindowHandle { get; set; }


        public bool IsInitialized { get; }


        public Renderer(WindowsFormsHost renderHost, IGLInvoker gl)
        {
            _glControl = new GLControl();
            _glControl.Paint += _glControl_Paint;
            _glControl.Dock = DockStyle.Fill;
            _glControl.MakeCurrent();
            renderHost.Child = _glControl;

            _gl = gl;
            _gl.EnableAlpha();
            _gl.ClearColor(100, 148, 237, 255);
        }


        private void _glControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Viewport(_glControl.Size);

            RenderBegin();

            Draw(_textures);

            RenderEnd();
        }


        public void Clear(byte red, byte green, byte blue, byte alpha)
        {
            _gl.ClearColor(red, green, blue, alpha);

            GL.Clear(ClearBufferMask.ColorBufferBit);
        }


        public void Init(IntPtr surfaceHandle)
        {
            throw new NotImplementedException();
        }


        public Texture LoadTexture(string path)
        {
            throw new NotImplementedException();
        }


        public void Render(Texture texture)
        {
            _currentTexture = texture;

            _glControl.Invalidate();
        }


        public void Render(Texture[] textures)
        {
            _textures = textures;

            _glControl.Invalidate();
        }


        public void RenderBegin()
        {
            Clear(100, 149, 237, 255);
        }


        public void RenderEnd()
        {
            _glControl.SwapBuffers();
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }


        private void Draw(Texture texture)
        {
            if (texture is null)
                return;

            texture.AdjustSize();
            AdjustAngle();

            var matrix = BuildTransformationMatrix(texture.X, texture.Y, texture.Width, texture.Height, texture.Size, _angle);

            UpdateTintColor(texture);

            //Update the transformation matrix on the GPU
            _gl.SetMat4Uniform(texture.Shaders[0].ProgramHandle, "transform", matrix);

            GL.BindVertexArray(texture.VA.VertexArrayHandle);
            texture.Use();
            texture.Shaders[0].Use();
            texture.VA.Bind();
            GL.DrawElements(PrimitiveType.Triangles, texture.Indices.Length, DrawElementsType.UnsignedInt, 0);

            texture.UnUse();
        }


        private void Draw(Texture[] textures)
        {
            if (textures is null)
                return;

            textures.ToList().ForEach(t => Draw(t));
        }


        private void AdjustAngle()
        {
            if (_angle >= 360f)
            {
                _increaseAngle = false;
            }
            else if (_angle <= 0f)
            {
                _increaseAngle = true;
            }

            _angle += _increaseAngle ? 1f : -1f;
        }


        private void UpdateTintColor(Texture texture)
        {
            var tintColor = new Vector4(0, 0, 0, 0);

            tintColor = tintColor.MapValues(0, 255, 0, 1);

            tintColor.W = 0.5f;

            //Update the tint color on the GPU
            _gl.SetVec4Uniform(texture.Shaders[0].ProgramHandle, "u_tintClr", tintColor);
        }


        private Matrix4 BuildTransformationMatrix(float x, float y, int width, int height, float size, float angle)
        {
            var scaleX = (float)width / _glControl.Width;
            var scaleY = (float)height / _glControl.Height;

            scaleX *= size;
            scaleY *= size;

            //NDC = Normalized Device Coordinates
            var ndcX = x.MapValue(0, _glControl.Width, -1f, 1f);
            var ndcY = y.MapValue(0, _glControl.Height, 1f, -1f);

            //NOTE: (+ degrees) rotates CCW and (- degress) rotates CW
            var angleRadians = MathHelper.DegreesToRadians(angle);

            //Invert angle to rotate CW instead of CCW
            angleRadians *= -1;

            var rotation = Matrix4.CreateRotationZ(angleRadians);
            var scale = Matrix4.CreateScale(scaleX, scaleY, 1f);
            var posMatrix = Matrix4.CreateTranslation(new Vector3(ndcX, ndcY, 0));


            return rotation * scale * posMatrix;
        }
    }
}
