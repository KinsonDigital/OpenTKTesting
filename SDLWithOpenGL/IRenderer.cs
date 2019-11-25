using System;

namespace SDLWithOpenGL
{
    /// <summary>
    /// Renders graphics to a render target.
    /// </summary>
    public interface IRenderer : IDisposable
    {
        #region Props
        /// <summary>
        /// Gets the pointer/window handler of the window to render the graphics to.
        /// </summary>
        IntPtr WindowHandle { get; set; }

        /// <summary>
        /// Gets a value indicating if the <see cref="IRenderer"/> has been initialized.
        /// </summary>
        bool IsInitialized { get; }
        #endregion


        #region Methods
        /// <summary>
        /// Initializes the renderer.
        /// </summary>
        /// <param name="surfaceHandle">The handle to the rendering surface.</param>
        void Init(IntPtr surfaceHandle);


        /// <summary>
        /// Loads texture file from disk and returns it as a <see cref="Particle{ITexture}"/>.
        /// </summary>
        /// <param name="path">The path to the texture file.</param>
        /// <returns></returns>
        Texture LoadTexture(string path);


        /// <summary>
        /// Begins the rendering process.
        /// </summary>
        /// <remarks>This method must be invoked first before the <see cref="Render(Particle{ParticleTexture})"/> method.</remarks>
        void RenderBegin();


        /// <summary>
        /// Renders the given <paramref name="particle"/> to the render target.
        /// Each render call will be batched.
        /// </summary>
        /// <param name="particle">The particle to render.</param>
        void Render(Texture particle);


        /// <summary>
        /// Ends the rendering process.  All of the graphics batched in the <see cref="Render(Particle{ParticleTexture})"/>
        /// calls will be rendered to the render target when this method is invoked.
        /// </summary>
        void RenderEnd();


        /// <summary>
        /// Clears the render surface to the given <paramref name="red"/>, <paramref name="green"/>,
        /// <paramref name="blue"/> and <paramref name="alpha"/> color components.
        /// </summary>
        /// <param name="red">The red color component.</param>
        /// <param name="green">The green color component.</param>
        /// <param name="blue">The blue color component.</param>
        /// <param name="alpha">The alpha color component.</param>
        void Clear(byte red, byte green, byte blue, byte alpha);
        #endregion
    }
}
