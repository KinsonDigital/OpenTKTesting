using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SDLWithOpenGL.Services;
using SDL2;

namespace SDLWithOpenGL.SDL2
{
    /// <summary>
    /// Renders graphics to a render target using <see cref="SDLInvoker"/>.
    /// </summary>
    public class SDLRenderer : IRenderer
    {
        #region Private Fields
        private readonly ISDLInvoker _sdl;
        private readonly IFileService _fileService;
        private SDLSafeHandle? _renderPtr;
        private bool _beginInvokedFirst;//Keeps track if the Begin() method has been invoked
        private bool _disposed;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="SDLRenderer"/>.
        /// <paramref name="sdl">Provides access to SDL related functionality.</paramref>
        /// <paramref name="fileService">Performs file related operations.</paramref>
        /// </summary>
        public SDLRenderer(ISDLInvoker sdl, IFileService fileService)
        {
            _sdl = sdl;
            _fileService = fileService;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the pointer/window handler of the window to render the graphics to.
        /// </summary>
        public IntPtr WindowHandle { get; set; }

        /// <summary>
        /// Gets a value indicating if the <see cref="SDLRenderer"/> has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Initializes the renderer.
        /// </summary>
        public void Init(IntPtr surfaceHandle)
        {
            IsInitialized = false;

            if (surfaceHandle == IntPtr.Zero)
                throw new Exception($"Surface handle cannot be zero! SDL_Error: {_sdl.GetError()}");
            else
            {
                if (_renderPtr != null && !_renderPtr.IsInvalid)
                    _sdl.DestroyRenderer(_renderPtr.DangerousGetHandle());

                _renderPtr?.Dispose();
                _renderPtr = new SDLSafeHandle(_sdl.CreateRenderer(surfaceHandle, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED));

                if (_renderPtr.IsInvalid)
                    throw new Exception($"Renderer could not be created! SDL Error: {_sdl.GetError()}");
                else
                {
                    //Initialize renderer color
                    var renderDrawClrResult = _sdl.SetRenderDrawColor(_renderPtr.DangerousGetHandle(), 48, 48, 48, 255);

                    if (renderDrawClrResult != 0)
                        throw new Exception(_sdl.GetError());

                    //Initialize PNG loading
                    _sdl.ImgInitPNG();
                }
            }

            IsInitialized = true;
        }


        /// <summary>
        /// Loads a texture file from disk using the given <paramref name="path"/> and returns it as a <see cref="Particle{ITexture}"/>.
        /// </summary>
        /// <param name="path">The path to the texture file.</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public Texture LoadTexture(string path)
        {
            if (_renderPtr is null || _renderPtr.IsInvalid)
                throw new Exception($"The renderer has not been initialized.  Use the '{nameof(Init)}()' method.");

            if (!_fileService.Exists(path))
                throw new FileNotFoundException("The particle file at the given path was not found.", path);

            //Load image at specified path
            var loadedSurface = _sdl.ImgLoad(path);

            if (loadedSurface == IntPtr.Zero)
            {
                throw new Exception($"Unable to load image at path '{path}'! SDL Error: {_sdl.GetError()}");
            }
            else
            {
                //Create texture from surface pixels
                var texturePtr = new SDLSafeHandle(_sdl.CreateTextureFromSurface(_renderPtr.DangerousGetHandle(), loadedSurface));

                if (texturePtr.IsInvalid)
                    throw new Exception($"Unable to create texture from '{path}'! SDL Error: {_sdl.GetError()}");

                var (queryTextureResult, format, access, width, height) = _sdl.QueryTexture(texturePtr.DangerousGetHandle());

                if (queryTextureResult != 0)
                    throw new Exception(_sdl.GetError());

                //Get rid of old loaded surface
                _sdl.FreeSurface(loadedSurface);


                return new Texture(_sdl, texturePtr.DangerousGetHandle(), width, height) { Name = Path.GetFileNameWithoutExtension(path) };
            }
        }


        /// <summary>
        /// Begins the rendering process.
        /// </summary>
        /// <remarks>This method must be invoked first before the <see cref="Render(Particle{ParticleTexture})"/> method.</remarks>
        public void RenderBegin()
        {
            Clear(48, 48, 48, 255);
            _beginInvokedFirst = true;
        }


        /// <summary>
        /// Rendeers the given <paramref name="particle"/> to the render target.
        /// Each render call will be batched.
        /// </summary>
        /// <param name="particle">The particle to render.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Render(Texture particle)
        {
            if (particle is null)
                throw new ArgumentNullException(nameof(particle), "Param cannot be null");

            if (_renderPtr is null || _renderPtr.IsInvalid)
                throw new Exception("Render pointer must not be null, zero or negative.");

            if (!_beginInvokedFirst)
                throw new Exception($"The '{nameof(RenderBegin)}()' method must be invoked first.");

            var textureOrigin = new SDL.SDL_Point()
            {
                x = particle.Width / 2,
                y = particle.Height / 2
            };

            var srcRect = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = particle.Width,
                h = particle.Height
            };

            var destRect = new SDL.SDL_Rect()
            {
                x = (int)(particle.Position.X - particle.Width / 2),//Texture X on screen
                y = (int)(particle.Position.Y - particle.Height / 2),//Texture Y on screen
                w = (int)(particle.Width * particle.Size),//Scaled occurding to size
                h = (int)(particle.Height * particle.Size)
            };

            var setBlendModeResult = _sdl.SetTextureBlendMode(particle.TexturePointer.DangerousGetHandle(), SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            if (setBlendModeResult != 0)
                throw new Exception(_sdl.GetError());

            var setColorModResult = _sdl.SetTextureColorMod(particle.TexturePointer.DangerousGetHandle(), particle.TintColor.R, particle.TintColor.G, particle.TintColor.B);

            if (setColorModResult != 0)
                throw new Exception(_sdl.GetError());

            var setTextureAlphaModResult = _sdl.SetTextureAlphaMod(particle.TexturePointer.DangerousGetHandle(), particle.TintColor.A);

            if (setTextureAlphaModResult != 0)
                throw new Exception(_sdl.GetError());

            var renderCopyExResult = _sdl.RenderCopyEx(_renderPtr.DangerousGetHandle(), particle.TexturePointer.DangerousGetHandle(), srcRect, destRect, particle.Angle, textureOrigin, SDL.SDL_RendererFlip.SDL_FLIP_NONE);

            if (renderCopyExResult != 0)
                throw new Exception(_sdl.GetError());
        }


        /// <summary>
        /// Ends the rendering process.  All of the graphics batched in the <see cref="Render(Particle{ParticleTexture})"/>
        /// calls will be rendered to the render target when this method is invoked.
        /// </summary>
        public void RenderEnd()
        {
            if (!(_renderPtr is null))
                _sdl.RenderPresent(_renderPtr.DangerousGetHandle());

            _beginInvokedFirst = false;
        }


        /// <summary>
        /// Clears the render surface to the given <paramref name="red"/>, <paramref name="green"/>,
        /// <paramref name="blue"/> and <paramref name="alpha"/> color components.
        /// </summary>
        /// <param name="red">The red color component.</param>
        /// <param name="green">The green color component.</param>
        /// <param name="blue">The blue color component.</param>
        /// <param name="alpha">The alpha color component.</param>
        public void Clear(byte red, byte green, byte blue, byte alpha)
        {
            if (_renderPtr is null)
                return;

            var drawColorResult = _sdl.SetRenderDrawColor(_renderPtr.DangerousGetHandle(), red, green, blue, alpha);

            if (drawColorResult != 0)
                throw new Exception(_sdl.GetError());

            var renderClearResult = _sdl.RenderClear(_renderPtr.DangerousGetHandle());

            if (renderClearResult != 0)
                throw new Exception(_sdl.GetError());
        }


        /// <summary>
        /// Disposes of the <see cref="SDLRenderer"/>.
        /// NOTE: Invoking <see cref="ShutDown"/> will also call this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Disposes of the internal resources if the given <paramref name="disposing"/> is true.
        /// </summary>
        /// <param name="disposing">True to dispose of internal resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            //Free managed resources
            if (disposing)
                _renderPtr?.Dispose();

            //Free unmanaged resources
            if (_renderPtr != null)
                _sdl.DestroyRenderer(_renderPtr.DangerousGetHandle());

            _sdl.ImageQuit();
            _sdl.Quit();

            _disposed = true;
        }
        #endregion
    }
}
