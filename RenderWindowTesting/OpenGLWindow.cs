using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RenderWindowTesting
{
    //TODO: Look into making the window properly disposable.  Probably in the overridden unload method

    /// <summary>
    /// Provides rendering surface window to render particle graphics to.
    /// </summary>
    public class OpenGLWindow : GameWindow
    {
        #region Private Fields
        private IGLInvoker _gl;
        private ParticleEngine<Texture> _particleEngine;

        //private readonly INativeMethods _nativeMethods;
        private bool _isInTaskbar = true;
        private bool _isAlwaysOnTop;
        private const string WINDOW_TITLE = "Particle Rendering Surface";
        private Point _previousMouseLocation;
        private Point _currentMouseLocation;
        private bool _disposed;
        private bool _shownExecuted;

        //This is how the textures are loaded in RenderEngine.
        private List<string> _texturePaths = new List<string>(new string[] { "Link.png" });
        private bool _beginInvoked;
        private float _elapsedMS;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="OpenGLWindow"/>.
        /// </summary>
        /// <param name="dispatcher">The dispatcher used to execute the size and location related events on the main UI thread.</param>
        /// <param name="sdl">Used to invoke SDL specific operations.</param>
        /// <param name="taskService">Used to manage async tasks.</param>
        /// <param name="nativeMethods">Used to access window specific API functionality.</param>
        public OpenGLWindow(IGLInvoker gl, ParticleEngine<Texture> particleEngine) : base(500, 500, GraphicsMode.Default, string.Empty)
        {
            ViewPortWidth = 500;
            ViewPortHeight = 500;

            _gl = gl;
            _particleEngine = particleEngine;

            _gl.EnableAlpha();

            _gl.ClearColor(100, 148, 237, 255);
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the left location of the left side of the window on the screen.
        /// </summary>
        public int Left
        {
            get => Location.X;
            set => Location = new Point(value, Location.Y);
        }

        /// <summary>
        /// Gets or sets the location of the top of the window on the screen.
        /// </summary>
        public int Top
        {
            get => Location.Y;
            set => Location = new Point(Location.X, value);
        }


        public static int ViewPortWidth { get; private set; }

        public static int ViewPortHeight { get; private set; }


        /// <summary>
        /// Gets or sets a value indicating if the <see cref="OpenGLWindow"/>
        /// has or does not have a border and title bar.
        /// </summary>
        //TODO: This can be removed
        public bool IsBorderless => WindowBorder == WindowBorder.Hidden;

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="OpenGLWindow"/>
        /// is or is not resizable.
        /// </summary>
        ////TODO: This can be removed
        public bool IsResizable => WindowBorder == WindowBorder.Resizable;

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="OpenGLWindow"/>
        /// is or is not always on top of other windows.
        /// </summary>
        //TODO: This might still be needed but if so, needs to use windows interop
        public bool IsAlwaysOnTop
        {
            get
            {
                return _isAlwaysOnTop;
            }
            set
            {
                //if (_isAlwaysOnTop == value)
                //    return;

                //var pinvokeHandle = GetPinvokeHandle();

                //if (value)
                //    _nativeMethods.EnableWindowTopMost(pinvokeHandle);
                //else
                //    _nativeMethods.DisableWindowTopMost(pinvokeHandle);

                //_isAlwaysOnTop = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="OpenGLWindow"/>
        /// will or will not be shown in the task bar.
        /// </summary>
        //TODO: Check this out and immplement.  This will probably still rely on windows interop
        public bool IsInTaskbar
        {
            get
            {
                return _isInTaskbar;
            }
            set
            {
                if (_isInTaskbar == value)
                    return;

                SetWindowTaskbarVisibility(value);

                _isInTaskbar = value;
            }
        }
        #endregion


        #region Public Methods
        protected override void OnLoad(EventArgs e)
        {
            _texturePaths.ForEach(path =>
            {
                _particleEngine.Add(new Texture(path));
            });

            base.OnLoad(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _elapsedMS += (float)e.Time * 1000f;

            if (_elapsedMS >= 16f)
            {
                _elapsedMS = 0;
            }

            _particleEngine.Particles.ToList().ForEach(p =>
            {
                var matrix = BuildTransformationMatrix(p.Position.X, p.Position.Y, p.Texture.Width, p.Texture.Height, p.Size, p.Angle);

                var tintColor = p.TintColor.ToVector4();

                tintColor = tintColor.MapValues(0, 255, 0, 1);

                tintColor.W = 0.5f;

                //Update the tint color on the GPU
                _gl.SetVec4Uniform(p.Texture.Shaders[0].ProgramHandle, "u_tintClr", tintColor);

                //Update the transformation matrix on the GPU
                _gl.SetMat4Uniform(p.Texture.Shaders[0].ProgramHandle, "transform", matrix);
            });

            _particleEngine.Update(new TimeSpan(0, 0, 0, 0, (int)(e.Time * 1000)));

            base.OnUpdateFrame(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Begin();

            _particleEngine.Particles.ToList().ForEach(p => Draw(p));

            End();

            base.OnRenderFrame(e);
        }


        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }


        protected override void OnResize(EventArgs e)
        {
            _particleEngine.Particles.ToList().ForEach(p =>
            {
                var matrix = BuildTransformationMatrix(p.Position.X, p.Position.Y, p.Texture.Width, p.Texture.Height, p.Size, p.Angle);

                //Update the transformation matrix on the GPU
                _gl.SetMat4Uniform(p.Texture.Shaders[0].ProgramHandle, "transform", matrix);
            });

            base.OnResize(e);
        }


        /// <summary>
        /// Hides the <see cref="OpenGLWindow"/>.
        /// </summary>
        //TODO: This can be removed later
        public void Hide() => Visible = false;


        /// <summary>
        /// Activates the <see cref="OpenGLWindow"/>.  This will bring the window
        /// in front of all the windows.
        /// </summary>
        //TODO: This might have to be done via windows interop
        public void Activate() { }


        /// <summary>
        /// Minimizes the <see cref="OpenGLWindow"/>.
        /// </summary>
        //TODO: This can be removed
        public void Minimize()
        {
            WindowState = WindowState.Minimized;
        }


        /// <summary>
        /// Maximizes the <see cref="OpenGLWindow"/>.
        /// </summary>
        //TODO: This can be removed
        public void Maximize()
        {
            WindowState = WindowState.Maximized;
        }


        /// <summary>
        /// Restores the <see cref="OpenGLWindow"/>.
        /// </summary>
        public void Restore()
        {
            WindowState = WindowState.Normal;
        }


        /// <summary>
        /// Gets the surface handle of the <see cref="OpenGLWindow"/>.
        /// </summary>
        /// <returns></returns>
        //TODO: This can be removed.  All rendering will now be done in the window
        internal IntPtr GetSurfaceHandle() => IntPtr.Zero;


        /// <summary>
        /// Closes the <see cref="OpenGLWindow"/>.
        /// </summary>
        //TODO: This can be removed
        public void Close()
        {
            Close();
        }
        #endregion


        #region Internal Methods
        /// <summary>
        /// Proceses only position and size related events of the SDL window.
        /// These events are checked every 1/8 of a second.
        /// </summary>
        internal void ProcessEvents()
        {
            //while (!_positionAndSizeEventTask.CancelRequested)
            //{
            //    _positionAndSizeEventTask.Wait(125);

            //    //Maintain a minimum width and height
            //    if (Width < MinWidth)
            //        Width = MinWidth;

            //    if (Height < MinHeight)
            //        Height = MinHeight;

            //    (bool hasPendingEvents, SDL_Event sdlEvent) pollResult;

            //    //Process all SDL window related events
            //    do
            //    {
            //        pollResult = _sdl.PollEvent();

            //        _dispatcher.BeginInvoke(() =>
            //        {
            //            switch (pollResult.sdlEvent.type)
            //            {
            //                case SDL_EventType.SDL_WINDOWEVENT:
            //                    switch (pollResult.sdlEvent.window.windowEvent)
            //                    {
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_NONE:
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
            //                            PostShow?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
            //                            PostHide?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
            //                            PostMinimize?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED:
            //                            PostMaximize?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
            //                            PostRestore?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
            //                            Close();
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
            //                            Moved?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
            //                            SizeChanged?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
            //                            FocusGained?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                        case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
            //                            FocusLost?.Invoke(this, EventArgs.Empty);
            //                            break;
            //                    }
            //                    break;
            //                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
            //                    MouseDown?.Invoke(this, new RenderSurfaceMouseEventArgs(pollResult.sdlEvent.motion.x, pollResult.sdlEvent.motion.y));
            //                    break;
            //                case SDL_EventType.SDL_MOUSEBUTTONUP:
            //                    MouseUp?.Invoke(this, new RenderSurfaceMouseEventArgs(pollResult.sdlEvent.motion.x, pollResult.sdlEvent.motion.y));
            //                    break;
            //                case SDL_EventType.SDL_MOUSEMOTION:
            //                    _currentMouseLocation = new Point(pollResult.sdlEvent.motion.x, pollResult.sdlEvent.motion.y);

            //                    //If the position of the mouse has changed since the last time it has been checked
            //                    if (_currentMouseLocation.X != _previousMouseLocation.X || _currentMouseLocation.Y != _previousMouseLocation.Y)
            //                        MouseMove?.Invoke(this, new RenderSurfaceMouseEventArgs(pollResult.sdlEvent.motion.x, pollResult.sdlEvent.motion.y));
            //                    break;
            //            }
            //        });
            //    }
            //    while (pollResult.hasPendingEvents);

            //    _previousMouseLocation = _currentMouseLocation;
            //}
        }
        #endregion


        #region Private Methods
        private void Draw(Particle<Texture> particle)
        {
            GL.BindVertexArray(particle.Texture.VA.VertexArrayHandle);
            particle.Texture.Use();
            particle.Texture.Shaders[0].Use();
            GL.DrawElements(PrimitiveType.Triangles, particle.Texture.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }


        /// <summary>
        /// Gets the windows related handle for pinvoke calls to windows native methods.
        /// </summary>
        /// <returns></returns>
        //TODO: This can be removed
        private IntPtr GetPinvokeHandle()
        {
            //var (success, winInfo) = _sdl.GetWindowWMInfo(SurfaceHandle.DangerousGetHandle());

            //if (success == SDL_bool.SDL_FALSE)
            //    throw new Exception($"Error getting SDL Window WM Info!! \n{_sdl.GetError()}");


            return IntPtr.Zero;
        }


        /// <summary>
        /// Shows or hides the window in the windows taskbar.
        /// </summary>
        /// <param name="visible">The visible state of the window.</param>
        private void SetWindowTaskbarVisibility(bool visible)
        {
            //if (visible)
            //    _nativeMethods.ShowInTaskbar(GetPinvokeHandle());
            //else
            //    _nativeMethods.HideInTaskbar(GetPinvokeHandle());
        }


        private void Begin()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _beginInvoked = true;
        }


        private void End()
        {
            if (!_beginInvoked)
                throw new Exception("Begin() must be invoked first.");

            SwapBuffers();

            _beginInvoked = false;
        }


        private Matrix4 BuildTransformationMatrix(float x, float y, int width, int height, float size, float angle)
        {
            var scaleX = (float)width / ViewPortWidth;
            var scaleY = (float)height / ViewPortHeight;

            scaleX *= size;
            scaleY *= size;

            //NDC = Normalized Device Coordinates
            var ndcX = x.MapValue(0, ViewPortWidth, -1f, 1f);
            var ndcY = y.MapValue(0, ViewPortHeight, 1f, -1f);

            //NOTE: (+ degrees) rotates CCW and (- degress) rotates CW
            var angleRadians = MathHelper.DegreesToRadians(angle);

            //Invert angle to rotate CW instead of CCW
            angleRadians *= -1;

            var rotation = Matrix4.CreateRotationZ(angleRadians);
            var scale = Matrix4.CreateScale(scaleX, scaleY, 1f);
            var posMatrix = Matrix4.CreateTranslation(new Vector3(ndcX, ndcY, 0));


            return rotation * scale * posMatrix;
        }
        #endregion
    }
}
