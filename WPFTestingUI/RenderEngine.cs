using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Platform;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using WPFTestingUI.OpenGL;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace WPFTestingUI
{
    public class RenderEngine
    {
        private Task _glWinTask;
        private CancellationTokenSource _glWinTaskTokenSrc;
        private Texture _texture;
        private IGLInvoker _gl;
        private bool _beginInvoked;
        private bool _increaseSize;
        private float _size = 1;
        private bool _increaseAngle;
        private float _angle;
        private readonly GLControl _glControl;


        public RenderEngine(WindowsFormsHost renderHost)
        {
            _glControl = new GLControl();
            _glControl.Paint += _glControl_Paint;
            _glControl.Dock = DockStyle.Fill;
            _glControl.MakeCurrent();
            renderHost.Child = _glControl;

            //_graphicsContext = new GraphicsContext(GraphicsMode.Default, windowInfo, 4, 0, GraphicsContextFlags.ForwardCompatible);
            //_graphicsContext.MakeCurrent(windowInfo);
            //_graphicsContext.LoadAll();

            _gl = new GLInvoker();

            _gl.EnableAlpha();
            _gl.ClearColor(100, 148, 237, 255);

            _texture = new Texture("Link.png");
        }


        public bool IsRunning { get; set; }


        public void StartEngine()
        {
            SetupTask();
        }


        public void StopEngine() => throw new NotImplementedException();


        public void Play() => IsRunning = true;


        public void Pause() => IsRunning = false;


        private void SetupTask()
        {
            _glWinTaskTokenSrc = new CancellationTokenSource();

            _glWinTask = new Task(Loop, _glWinTaskTokenSrc.Token);

            _glWinTask.Start();
        }


        private void Loop()
        {
            while(!_glWinTaskTokenSrc.IsCancellationRequested)
            {
                _glWinTask.Wait(60);

                //Calling this invokes the control paint method
                _glControl.Invalidate();
            }
        }


        private void _glControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Viewport(_glControl.Size);

            Update();

            Render();
        }


        private void Update()
        {
            AdjustSize();
            AdjustAngle();

            var matrix = BuildTransformationMatrix(100, 100, 50, 64, _size, _angle);

            var tintColor = new Vector4(0, 0, 0, 0);

            tintColor = tintColor.MapValues(0, 255, 0, 1);

            tintColor.W = 0.5f;

            //Update the tint color on the GPU
            _gl.SetVec4Uniform(_texture.Shaders[0].ProgramHandle, "u_tintClr", tintColor);

            //Update the transformation matrix on the GPU
            _gl.SetMat4Uniform(_texture.Shaders[0].ProgramHandle, "transform", matrix);
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


        private void AdjustSize()
        {
            if (_size >= 2f)
            {
                _increaseSize = false;
            }
            else if(_size <= 0.5f)
            {
                _increaseSize = true;
            }

            _size += _increaseSize ? 0.01f : -0.01f;
        }


        private void Render()
        {
            if (!IsRunning)
                return;

            Begin();

            Draw(_texture);

            End();
        }




        private void Begin()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _beginInvoked = true;
        }


        private void Draw(Texture texture)
        {
            GL.BindVertexArray(texture.VA.VertexArrayHandle);
            texture.Use();
            texture.Shaders[0].Use();
            GL.DrawElements(PrimitiveType.Triangles, texture.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }


        private void End()
        {
            if (!_beginInvoked)
                throw new Exception("Begin() must be invoked first.");

            _glControl.SwapBuffers();

            _beginInvoked = false;
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
