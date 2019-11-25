using Microsoft.Win32.SafeHandles;
using SDLWithOpenGL.SDL2;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;

namespace SDLWithOpenGL
{
    /// <summary>
    /// Wrapper class for a window related pointer/handle to
    /// provide a safe way of releasing the handle.
    /// </summary>
    public sealed class WinSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Private Fields
        private readonly ISDLInvoker _sdl;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance <see cref="WinSafeHandle"/>.
        /// </summary>
        /// <param name="handle">The handle/pointer to use for SDL operations.</param>
        public WinSafeHandle(ISDLInvoker sdl, IntPtr handle) : base(true)
        {
            _sdl = sdl;
            SetHandle(handle);
        }
        #endregion


        #region Protected Methods
        /// <summary>
        /// Frees the handle.
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected override bool ReleaseHandle()
        {
            try
            {
                _sdl.DestroyWindow(handle);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
