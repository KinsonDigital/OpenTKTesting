using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;

namespace SDLWithOpenGL.SDL2
{
    /// <summary>
    /// Wrapper class for SDL related pointers and handles for 
    /// SDL related operations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class SDLSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance <see cref="SDLSafeHandle"/>.
        /// </summary>
        /// <param name="handle">The handle/pointer to use for SDL operations.</param>
        public SDLSafeHandle(IntPtr handle) : base(true) => SetHandle(handle);
        #endregion


        #region Props
        /// <summary>
        /// Gets a value that indicates whether the handle is invalid.
        /// </summary>
        public override bool IsInvalid => handle == IntPtr.Zero;
        #endregion


        #region Protected Methods
        /// <summary>
        /// Frees the handle.
        /// </summary>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle() => true;
        #endregion
    }
}
