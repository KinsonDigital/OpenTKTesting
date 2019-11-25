using SDLWithOpenGL.SDL2;
using SDLWithOpenGL.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using static SDL2.SDL;

namespace SDLWithOpenGL
{
    /// <summary>
    /// Provides rendering surface window to render particle graphics to.
    /// </summary>
    public class RenderSurfaceWindow : IDisposable
    {
        #region Public Events
        /// <summary>
        /// Occurs when the window gains focus.
        /// </summary>
        public event EventHandler<EventArgs>? FocusGained;

        /// <summary>
        /// Occurs when the window loses focus.
        /// </summary>
        public event EventHandler<EventArgs>? FocusLost;

        /// <summary>
        /// Occurs when the size of the window has changed.
        /// </summary>
        public event EventHandler<EventArgs>? SizeChanged;

        /// <summary>
        /// Occurs when the window has been moved.
        /// </summary>
        public event EventHandler<EventArgs>? Moved;

        /// <summary>
        /// Occurs right before the window closes.
        /// </summary>
        public event EventHandler<EventArgs>? PreClose;

        /// <summary>
        /// Occurs right after the window closes.
        /// </summary>
        public event EventHandler<EventArgs>? PostClose;

        /// <summary>
        /// Occurs right before the window minimizes.
        /// </summary>
        public event EventHandler<EventArgs>? PreMinimize;

        /// <summary>
        /// Occurs right after the window minimizes.
        /// </summary>
        public event EventHandler<EventArgs>? PostMinimize;

        /// <summary>
        /// Occurs right before the window maximizes.
        /// </summary>
        public event EventHandler<EventArgs>? PreMaximize;
        
        /// <summary>
        /// Occurs right after the window maximizes.
        /// </summary>
        public event EventHandler<EventArgs>? PostMaximize;

        /// <summary>
        /// Occurs right before the window is restored.
        /// </summary>
        public event EventHandler<EventArgs>? PreRestore;

        /// <summary>
        /// Occurs right after the window is restored.
        /// </summary>
        public event EventHandler<EventArgs>? PostRestore;

        /// <summary>
        /// Occurs right before the window is hidden.
        /// </summary>
        public event EventHandler<EventArgs>? PreHide;

        /// <summary>
        /// Occurs right after the window is hidden.
        /// </summary>
        public event EventHandler<EventArgs>? PostHide;

        /// <summary>
        /// Occurs right before the window is shown.
        /// </summary>
        public event EventHandler<EventArgs>? PreShow;

        /// <summary>
        /// Occurs right after the window is shown.
        /// </summary>
        public event EventHandler<EventArgs>? PostShow;

        /// <summary>
        /// Occurs when the mouse has moved over the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        public event EventHandler<EventArgs>? MouseMove;

        /// <summary>
        /// Occurs when any mouse button is pressed into the down position.
        /// </summary>
        public event EventHandler<EventArgs>? MouseDown;

        /// <summary>
        /// Occurs when any mouse button is released to the up position.
        /// </summary>
        public event EventHandler<EventArgs>? MouseUp;
        #endregion


        #region Private Fields
        private readonly ITaskManagerService _positionAndSizeEventTask;
        private readonly ISDLInvoker _sdl;
        private readonly INativeMethods _nativeMethods;
        private bool _isInTaskbar = true;
        private bool _isAlwaysOnTop;
        private const string WINDOW_TITLE = "Particle Rendering Surface";
        private Point _previousMouseLocation;
        private Point _currentMouseLocation;
        private bool _disposed;
        private bool _shownExecuted;


        //GL Stuff
        private int gProgramID;
        private int gVertexPos2DLocation = -1;
        private uint gVBO;
        private uint gIBO;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        /// <param name="dispatcher">The dispatcher used to execute the size and location related events on the main UI thread.</param>
        /// <param name="sdl">Used to invoke SDL specific operations.</param>
        /// <param name="taskService">Used to manage async tasks.</param>
        /// <param name="nativeMethods">Used to access window specific API functionality.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public RenderSurfaceWindow(ISDLInvoker sdl, ITaskManagerService taskService, INativeMethods nativeMethods)
        {
            _sdl = sdl ?? throw new ArgumentNullException(nameof(sdl));
            _nativeMethods = nativeMethods;

            _positionAndSizeEventTask = taskService;

            SurfaceHandle = new WinSafeHandle(_sdl, IntPtr.Zero);

            //Set texture filtering to linear
            if (_sdl.SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "0") == SDL_bool.SDL_FALSE)
                throw new Exception("Warning: Linear texture filtering not enabled!");
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the surface handle.
        /// </summary>
        internal WinSafeHandle SurfaceHandle { get; set; }

        /// <summary>
        /// Gets or sets the left location of the left side of the window on the screen.
        /// </summary>
        public int Left
        {
            get
            {
                var (x, _) = _sdl.GetWindowPosition(SurfaceHandle.DangerousGetHandle());


                return x;
            }
            set
            {
                if (SurfaceHandle.IsInvalid)
                    return;

                var (x, y) = _sdl.GetWindowPosition(SurfaceHandle.DangerousGetHandle());

                //Only set the position if it has changed.  This prevents the
                //position changed SDL event from firing.
                if (value != x)
                    _sdl.SetWindowPosition(SurfaceHandle.DangerousGetHandle(), value, y);
            }
        }

        /// <summary>
        /// Gets or sets the location of the top of the window on the screen.
        /// </summary>
        public int Top
        {
            get
            {
                var (_, y) = _sdl.GetWindowPosition(SurfaceHandle.DangerousGetHandle());


                return y;
            }
            set
            {
                if (SurfaceHandle.IsInvalid)
                    return;

                var (x, y) = _sdl.GetWindowPosition(SurfaceHandle.DangerousGetHandle());

                //Only set the position if it has changed.  This prevents the
                //position changed SDL event from firing.
                if (value != y)
                    _sdl.SetWindowPosition(SurfaceHandle.DangerousGetHandle(), x, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the <see cref="RenderSurfaceWindow"/> window.
        /// </summary>
        public int Width
        {
            get
            {
                var (w, _) = _sdl.GetWindowSize(SurfaceHandle.DangerousGetHandle());


                return w;
            }
            set
            {
                if (SurfaceHandle.IsInvalid)
                    return;

                var (w, h) = _sdl.GetWindowSize(SurfaceHandle.DangerousGetHandle());

                //Only set the size if it has changed.  This prevents the
                //size changed SDL event from firing.
                if (value != w)
                {
                    _sdl.SetWindowSize(SurfaceHandle.DangerousGetHandle(), value <= MinWidth ? MinWidth : value, h);
                    Clear();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the <see cref="RenderSurfaceWindow"/> window.
        /// </summary>
        public int Height
        {
            get
            {
                var (_, h) = _sdl.GetWindowSize(SurfaceHandle.DangerousGetHandle());


                return h;
            }
            set
            {
                if (SurfaceHandle.IsInvalid)
                    return;

                var (w, h) = _sdl.GetWindowSize(SurfaceHandle.DangerousGetHandle());

                //Only set the size if it has changed.  This prevents the
                //size changed SDL event from firing.
                if (value != h)
                {
                    _sdl.SetWindowSize(SurfaceHandle.DangerousGetHandle(), w, value <= MinHeight ? MinHeight : value);
                    Clear();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum width that the <see cref="RenderSurfaceWindow"/> can be set to.
        /// </summary>
        public int MinWidth { get; set; } = 300;

        /// <summary>
        /// Gets or sets the minimum height that the <see cref="RenderSurfaceWindow"/> can be set to.
        /// </summary>
        public int MinHeight { get; set; } = 300;

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="RenderSurfaceWindow"/>
        /// has or does not have a border and title bar.
        /// </summary>
        public bool IsBorderless
        {
            get => (_sdl.GetWindowFlags(SurfaceHandle.DangerousGetHandle()) & (uint)SDL_WindowFlags.SDL_WINDOW_BORDERLESS) == (uint)SDL_WindowFlags.SDL_WINDOW_BORDERLESS;
            set => _sdl.SetWindowBordered(SurfaceHandle.DangerousGetHandle(), value ? SDL_bool.SDL_FALSE : SDL_bool.SDL_TRUE);
        }

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="RenderSurfaceWindow"/>
        /// is or is not resizable.
        /// </summary>
        public bool IsResizable
        {
            get => (_sdl.GetWindowFlags(SurfaceHandle.DangerousGetHandle()) & (uint)SDL_WindowFlags.SDL_WINDOW_RESIZABLE) == (uint)SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            set => _sdl.SetWindowResizable(SurfaceHandle.DangerousGetHandle(), value ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="RenderSurfaceWindow"/>
        /// is or is not always on top of other windows.
        /// </summary>
        public bool IsAlwaysOnTop
        {
            get
            {
                return _isAlwaysOnTop;
            }
            set
            {
                if (_isAlwaysOnTop == value)
                    return;

                var pinvokeHandle = GetPinvokeHandle();

                if (value)
                    _nativeMethods.EnableWindowTopMost(pinvokeHandle);
                else
                    _nativeMethods.DisableWindowTopMost(pinvokeHandle);

                _isAlwaysOnTop = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="RenderSurfaceWindow"/>
        /// will or will not be shown in the task bar.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="RenderSurfaceWindow"/>
        /// is visible or not visible.
        /// </summary>
        public bool Visible
        {
            get => (_sdl.GetWindowFlags(SurfaceHandle.DangerousGetHandle()) & (uint)SDL_WindowFlags.SDL_WINDOW_SHOWN) == (uint)SDL_WindowFlags.SDL_WINDOW_SHOWN;
            set
            {
                if (value)
                {
                    PreShow?.Invoke(this, EventArgs.Empty);
                    _sdl.ShowWindow(SurfaceHandle.DangerousGetHandle());
                }
                else
                {
                    PreHide?.Invoke(this, EventArgs.Empty);
                    _sdl.HideWindow(SurfaceHandle.DangerousGetHandle());
                }
            }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Shows the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Show()
        {
            // If the shown method has already been invokded
            if (_shownExecuted)
            {
                if (Visible)
                    return;
                else
                {
                    Visible = true;
                    return;
                }
            }

            try
            {
                //Initialize SDL
                _sdl.Init(SDL_INIT_VIDEO);

                //SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
                //SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 1);
                //SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);

                //if (SDL_GL_SetSwapInterval(1) < 0)
                //    throw new Exception($"Warning: Unable to set VSync! SDL Error: {SDL_GetError()}");

                //if (!InitGL())
                //{
                //    throw new Exception("Unable to initialize OpenGL!");
                //}

                SurfaceHandle.Dispose();

                if (_isInTaskbar && _isAlwaysOnTop)
                {
                    SurfaceHandle = new WinSafeHandle(_sdl, _sdl.CreateWindow(WINDOW_TITLE, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
                        640, 480, SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_SKIP_TASKBAR | SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP));
                }
                else if(_isInTaskbar && !_isAlwaysOnTop)
                {
                    SurfaceHandle = new WinSafeHandle(_sdl, _sdl.CreateWindow(WINDOW_TITLE, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
                        640, 480, SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE));
                }
                else if(!_isInTaskbar && _isAlwaysOnTop)
                {
                    SurfaceHandle = new WinSafeHandle(_sdl, _sdl.CreateWindow(WINDOW_TITLE, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
                        640, 480, SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_SKIP_TASKBAR));
                }
                else if(!_isInTaskbar && !_isAlwaysOnTop)
                {
                    SurfaceHandle = new WinSafeHandle(_sdl, _sdl.CreateWindow(WINDOW_TITLE, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
                        640, 480, SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_SKIP_TASKBAR | SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP));
                }


                if (SurfaceHandle.IsInvalid)
                    throw new Exception($"Window could not be created using the invalid surface handle. \n\nSDL_Error: {_sdl.GetError()}");

                SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 2);
                SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 1);
                var glContext = SDL_GL_CreateContext(SurfaceHandle.DangerousGetHandle());

                _positionAndSizeEventTask.StartTask(ProcessEvents);
            }
            catch (Exception ex)
            {
                throw new Exception("default-message", ex);
            }

            _shownExecuted = true;
        }

        private bool InitGL()
        {
            // Success flag
            bool success = true;

            //Generate program
            gProgramID = GL.CreateProgram();

            //Create vertex shader
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);

            //Get vertex source
            string[] vertexShaderSource = new[]
            {
                "#version 140\nin vec2 LVertexPos2D; void main() { gl_Position = vec4( LVertexPos2D.x, LVertexPos2D.y, 0, 1 ); }"
            };

            //WARNING!! - The shader lengths param might be an issue here
            var shaderLengths = vertexShaderSource.Length;
            //Set vertex source
            GL.ShaderSource(vertexShader, 1, vertexShaderSource, ref shaderLengths);

            //Compile vertex source
            GL.CompileShader(vertexShader);

            //Check vertex shader for errors
            var vertexShaderCompiled = 0;

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out vertexShaderCompiled);

            if (vertexShaderCompiled != 1)
            {
                throw new Exception("Unable to compile vertex shader %d!");
                success = false;
            }
            else
            {
                //Attach vertex shader to program
                GL.AttachShader(gProgramID, vertexShader);


                //Create fragment shader
                var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

                //Get fragment source
                string[] fragmentShaderSource = new[]
                {
                    "#version 140\nout vec4 LFragment; void main() { LFragment = vec4( 1.0, 1.0, 1.0, 1.0 ); }"
                };

                var fragmentShaderLengths = fragmentShaderSource.Length;

                //Set fragment source
                GL.ShaderSource(fragmentShader, 1, fragmentShaderSource, ref fragmentShaderLengths);

                //Compile fragment source
                GL.CompileShader(fragmentShader);

                //Check fragment shader for errors
                var fShaderCompiled = 0;

                var fragmentShaderCompiled = 0;

                GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out fragmentShaderCompiled);

                if (fShaderCompiled != 1)
                {
                    throw new Exception("Unable to compile fragment shader %d!");
                    success = false;
                }
                else
                {
                    //Attach fragment shader to program
                    GL.AttachShader(gProgramID, fragmentShader);


                    //Link program
                    GL.LinkProgram(gProgramID);

                    //Check for errors
                    var programSuccess = 1;
                    GL.GetProgram(gProgramID, GetProgramParameterName.LinkStatus, out programSuccess);

                    if (programSuccess != 1)
                    {
                        throw new Exception("Error linking program %d!");
                        success = false;
                    }
                    else
                    {
                        //Get vertex attribute location
                        gVertexPos2DLocation = GL.GetAttribLocation(gProgramID, "LVertexPos2D");
                        if (gVertexPos2DLocation == -1)
                        {
                            throw new Exception("LVertexPos2D is not a valid GL.sl program variable!");
                            success = false;
                        }
                        else
                        {
                            //Initialize clear color
                            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

                            //VBO data
                            float[] vertexData =
                            {
                                    -0.5f, -0.5f,
                                     0.5f, -0.5f,
                                     0.5f,  0.5f,
                                    -0.5f,  0.5f
                                };

                            //IBO data
                            var indexData = new[] { 0U, 1U, 2U, 3U };

                            //Create VBO
                            GL.GenBuffers(1, out gVBO);
                            GL.BindBuffer(BufferTarget.ArrayBuffer, gVBO);
                            GL.BufferData(BufferTarget.ArrayBuffer, 2 * 4 * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

                            //Create IBO
                            GL.GenBuffers(1, out gIBO);
                            GL.BindBuffer(BufferTarget.ArrayBuffer, gIBO);
                            GL.BufferData(BufferTarget.ArrayBuffer, 4 * sizeof(uint), indexData, BufferUsageHint.StaticDraw);
                        }
                    }
                }
            }


            return success;
        }


        /// <summary>
        /// Hides the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        public void Hide() => _sdl.HideWindow(SurfaceHandle.DangerousGetHandle());


        /// <summary>
        /// Activates the <see cref="RenderSurfaceWindow"/>.  This will bring the window
        /// in front of all the windows.
        /// </summary>
        public void Activate() => _sdl.RaiseWindow(SurfaceHandle.DangerousGetHandle());


        /// <summary>
        /// Minimizes the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        public void Minimize()
        {
            PreMinimize?.Invoke(this, EventArgs.Empty);
            _sdl.MinimizeWindow(SurfaceHandle.DangerousGetHandle());
        }


        /// <summary>
        /// Maximizes the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        public void Maximize()
        {
            PreMaximize?.Invoke(this, EventArgs.Empty);
            _sdl.MaximizeWindow(SurfaceHandle.DangerousGetHandle());
        }
        

        /// <summary>
        /// Restores the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        public void Restore()
        {
            PreRestore?.Invoke(this, EventArgs.Empty);

            _sdl.RestoreWindow(SurfaceHandle.DangerousGetHandle());
        }


        /// <summary>
        /// Gets the surface handle of the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        /// <returns></returns>
        internal IntPtr GetSurfaceHandle() => SurfaceHandle.DangerousGetHandle();
        

        /// <summary>
        /// Closes the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        public void Close()
        {
            PreClose?.Invoke(this, EventArgs.Empty);

            _positionAndSizeEventTask.StopTask();
            _positionAndSizeEventTask.Wait();

            SurfaceHandle.Dispose();

            PostClose?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Disposes of the <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region Internal Methods
        /// <summary>
        /// Proceses only position and size related events of the SDL window.
        /// These events are checked every 1/8 of a second.
        /// </summary>
        internal void ProcessEvents()
        {
            while (!_positionAndSizeEventTask.CancelRequested)
            {
                _positionAndSizeEventTask.Wait(125);

                //Maintain a minimum width and height
                //if (Width < MinWidth)
                //    Width = MinWidth;

                //if (Height < MinHeight)
                //    Height = MinHeight;

                (bool hasPendingEvents, SDL_Event sdlEvent) pollResult;

                //Process all SDL window related events
                do
                {
                    pollResult = _sdl.PollEvent();

                    switch (pollResult.sdlEvent.type)
                    {
                        case SDL_EventType.SDL_WINDOWEVENT:
                            switch (pollResult.sdlEvent.window.windowEvent)
                            {
                                case SDL_WindowEventID.SDL_WINDOWEVENT_NONE:
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
                                    PostShow?.Invoke(this, EventArgs.Empty);
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
                                    PostHide?.Invoke(this, EventArgs.Empty);
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
                                    PostMinimize?.Invoke(this, EventArgs.Empty);
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED:
                                    PostMaximize?.Invoke(this, EventArgs.Empty);
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
                                    PostRestore?.Invoke(this, EventArgs.Empty);
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                                    Close();
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                                    Moved?.Invoke(this, EventArgs.Empty);
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                                    var width = pollResult.sdlEvent.window.data1;
                                    var height = pollResult.sdlEvent.window.data2;

                                    //The SDL size changed event can fire even though the size has not actually changed.
                                    //Only fire the SizeChanged() event if the width or height has actually changed.
                                    if (width != Width || height != Height)
                                        SizeChanged?.Invoke(this, EventArgs.Empty);
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                                    FocusGained?.Invoke(this, EventArgs.Empty);
                                    break;
                                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                                    FocusLost?.Invoke(this, EventArgs.Empty);
                                    break;
                            }
                            break;
                        case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            MouseDown?.Invoke(this, new RenderSurfaceMouseEventArgs(pollResult.sdlEvent.motion.x, pollResult.sdlEvent.motion.y));
                            break;
                        case SDL_EventType.SDL_MOUSEBUTTONUP:
                            MouseUp?.Invoke(this, new RenderSurfaceMouseEventArgs(pollResult.sdlEvent.motion.x, pollResult.sdlEvent.motion.y));
                            break;
                        case SDL_EventType.SDL_MOUSEMOTION:
                            _currentMouseLocation = new Point(pollResult.sdlEvent.motion.x, pollResult.sdlEvent.motion.y);

                            //If the position of the mouse has changed since the last time it has been checked
                            if (_currentMouseLocation.X != _previousMouseLocation.X || _currentMouseLocation.Y != _previousMouseLocation.Y)
                                MouseMove?.Invoke(this, new RenderSurfaceMouseEventArgs(pollResult.sdlEvent.motion.x, pollResult.sdlEvent.motion.y));
                            break;
                    }
                }
                while (pollResult.hasPendingEvents);

                _previousMouseLocation = _currentMouseLocation;
            }
        }
        #endregion


        #region Protected Methods
        /// <summary>
        /// Disposes of the internal resources if the given <paramref name="disposing"/> is true.
        /// </summary>
        /// <param name="disposing">True to dispose of internal resources.</param>
        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            //Free managed resources
            if (disposing)
            {
                _positionAndSizeEventTask.StopTask();
                _positionAndSizeEventTask.Dispose();
                SurfaceHandle.Dispose();
            }

            _disposed = true;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Gets the windows related handle for pinvoke calls to windows native methods.
        /// </summary>
        /// <returns></returns>
        private IntPtr GetPinvokeHandle()
        {
            var (success, winInfo) = _sdl.GetWindowWMInfo(SurfaceHandle.DangerousGetHandle());

            if (success == SDL_bool.SDL_FALSE)
                throw new Exception($"Error getting SDL Window WM Info!! \n{_sdl.GetError()}");


            return winInfo.info.win.window;
        }


        /// <summary>
        /// Clears the surface of the window to prevent the display of
        /// graphical artifacts after resizing when rendering is not occuring.
        /// </summary>
        private void Clear()
        {
            var destroyRenderer = false;

            var renderer = _sdl.GetRenderer(SurfaceHandle.DangerousGetHandle());

            //Use the renderer for the window if one already exists.
            if (renderer == IntPtr.Zero)
            {
                renderer = CreateRenderer(SurfaceHandle.DangerousGetHandle());
                destroyRenderer = true;
            }

            SetDrawColor(renderer);

            try
            {
                var renderClearResult = _sdl.RenderClear(renderer);

                _sdl.RenderPresent(renderer);

                //If the render did not exist already before running this method,
                //destory the render.  This is to prevent any issues with other
                //code trying to create a renderer for this window.
                if (destroyRenderer)
                    _sdl.DestroyRenderer(renderer);
            }
            catch (Exception ex)
            {
                throw new Exception("default-message", ex);
            }
        }


        /// <summary>
        /// Creates a renderer for the given surface handle.
        /// </summary>
        /// <param name="surfaceHandle">The handle to the surface to create the renderer for.</param>
        /// <returns></returns>
        private IntPtr CreateRenderer(IntPtr surfaceHandle)
        {
            IntPtr renderPtr;

            if (surfaceHandle == IntPtr.Zero)
                throw new Exception($"Surface handle cannot be zero! SDL_Error: {_sdl.GetError()}");
            else
            {
                //Create vsynced renderer for window
                var renderFlags = SDL_RendererFlags.SDL_RENDERER_ACCELERATED;

                try
                {
                    renderPtr = _sdl.CreateRenderer(surfaceHandle, -1, renderFlags);

                    //Initialize renderer color
                    SetDrawColor(renderPtr);

                    //Initialize PNG loading
                    _sdl.ImgInitPNG();
                }
                catch (Exception ex)
                {
                    throw new Exception("default-message", ex);
                }
            }


            return renderPtr;
        }


        /// <summary>
        /// Sets the draw color.
        /// </summary>
        private void SetDrawColor(IntPtr rendererPtr)
        {
            var drawColorResult = _sdl.SetRenderDrawColor(rendererPtr, 48, 48, 48, 255);

            if (drawColorResult != 0)
                throw new Exception(_sdl.GetError());
        }


        /// <summary>
        /// Shows or hides the window in the windows taskbar.
        /// </summary>
        /// <param name="visible">The visible state of the window.</param>
        private void SetWindowTaskbarVisibility(bool visible)
        {
            if (visible)
                _nativeMethods.ShowInTaskbar(GetPinvokeHandle());
            else
                _nativeMethods.HideInTaskbar(GetPinvokeHandle());
        }
        #endregion
    }
}
