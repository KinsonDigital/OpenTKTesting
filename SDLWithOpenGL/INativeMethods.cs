using System;

namespace SDLWithOpenGL
{
    /// <summary>
    /// Provides access to native API methods.
    /// </summary>
    public interface INativeMethods
    {
        #region Methods
        /// <summary>
        /// Enables the window using the given <paramref name="windowHandle"/> as a top most window.
        /// </summary>
        /// <param name="windowHandle">The handle/pointer to the window.</param>
        void EnableWindowTopMost(IntPtr windowHandle);


        /// <summary>
        /// Disables the window using the given <paramref name="windowHandle"/> as the top most window.
        /// </summary>
        /// <param name="windowHandle">The handle/pointer to the window.</param>
        void DisableWindowTopMost(IntPtr windowHandle);


        /// <summary>
        /// Shows the window in the taskbar using the given <paramref name="windowHandle"/>.
        /// </summary>
        /// <param name="windowHandle">The handle/pointer to the window.</param>
        void ShowInTaskbar(IntPtr windowHandle);


        /// <summary>
        /// Hides the window from the taskbar using the given <paramref name="windowHandle"/>.
        /// </summary>
        /// <param name="windowHandle">The handle/pointer to the window.</param>
        void HideInTaskbar(IntPtr windowHandle);
        #endregion
    }
}