using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SDLWithOpenGL
{
    /// <summary>
    /// Provides access to native API methods.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NativeMethods : INativeMethods
    {
        #region PInvoke Functions
        /// <summary>
        ///  Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered
        /// according to their appearance on the screen. The topmost window receives the highest rank and is the first window
        /// in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in 
        /// the Z order. This parameter must be a window handle or one of the following values.</param>
        /// <param name="X">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="Y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="uFlags">The window sizing and positioning flags.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


        /// <summary>
        /// Changes an attribute of the specified window. The function also sets a value at the specified offset in the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs. The SetWindowLongPtr function
        /// fails if the process that owns the window specified by the hWnd parameter is at a higher process privilege in the UIPI
        /// hierarchy than the process the calling thread resides in.
        /// Windows XP/2000:   The SetWindowLongPtr function fails if the window specified by the hWnd parameter does not belong to the same process as the calling thread.
        /// </param>
        /// <param name="nIndex">The zero-based offset to the value to be set. Valid values are in the range zero through the number of
        /// bytes of extra window memory, minus the size of a LONG_PTR. To set any other value, specify one of the following values.</param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int SetWindowLongPtrA(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion


        #region Private Fields
        private const string TASKBAR_EXCEPTION_MESSAGE = "There was an issue trying to show or hide the window in the taskbar.";

        #region PInvoke Constants
        /// <summary>
        /// Places the window above all non-topmost windows. The window maintains its topmost position
        /// even when it is deactivated.
        /// </summary>
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). This
        /// flag has no effect if the window is already a non-topmost window.
        /// </summary>
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        /// <summary>
        /// Sets a new extended window style.
        /// </summary>
        private const int GWL_EXSTYLE = -0x14;

        /// <summary>
        /// Forces a top-level window onto the taskbar when the window is visible.
        /// </summary>
        private const int WS_EX_APPWINDOW = 0x40000;

        /// <summary>
        /// A top-level window created with this style does not become the foreground window when the user clicks it. The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
        /// The window should not be activated through programmatic access or via keyboard navigation by accessible technology, such as Narrator.
        /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
        /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
        /// </summary>
        private const int WS_EX_NOACTIVATE = 0x08000000;

        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        private const uint SWP_NOSIZE = 0x0001;

        /// <summary>
        /// Retains the current position (ignores X and Y parameters).
        /// </summary>
        private const uint SWP_NOMOVE = 0x0002;

        /// <summary>
        /// Does not activate the window. If this flag is not set, the window is activated and moved to
        /// the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
        /// parameter).
        /// </summary>
        private const uint SWP_NOACTIVATE = 0x0010;

        /// <summary>
        /// Combination of the required flags to only change the top most window behavior.
        /// </summary>
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE;
        #endregion
        #endregion


        #region Public Methods
        /// <summary>
        /// Enables the window using the given <paramref name="windowHandle"/> as a top most window.
        /// </summary>
        /// <param name="windowHandle">The handle/pointer to the window.</param>
        public void EnableWindowTopMost(IntPtr windowHandle) => SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);


        /// <summary>
        /// Disables the window using the given <paramref name="windowHandle"/> as the top most window.
        /// </summary>
        /// <param name="windowHandle">The handle/pointer to the window.</param>
        public void DisableWindowTopMost(IntPtr windowHandle) => SetWindowPos(windowHandle, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);


        /// <summary>
        /// Shows the window in the taskbar using the given <paramref name="windowHandle"/>.
        /// </summary>
        /// <param name="windowHandle">The handle/pointer to the window.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void ShowInTaskbar(IntPtr windowHandle)
        {
            var result = SetWindowLongPtrA(windowHandle, GWL_EXSTYLE, WS_EX_APPWINDOW);

            if (result == 0)
                throw new Exception(TASKBAR_EXCEPTION_MESSAGE);
        }


        /// <summary>
        /// Hides the window from the taskbar using the given <paramref name="windowHandle"/>.
        /// </summary>
        /// <param name="windowHandle">The handle/pointer to the window.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void HideInTaskbar(IntPtr windowHandle)
        {
            var result = SetWindowLongPtrA(windowHandle, GWL_EXSTYLE, WS_EX_NOACTIVATE);

            if (result == 0)
                throw new Exception(TASKBAR_EXCEPTION_MESSAGE);
        }
        #endregion
    }
}
