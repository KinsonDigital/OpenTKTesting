using System;

namespace SDLWithOpenGL
{
    /// <summary>
    /// Holds information about a render surface mouse event.
    /// </summary>
    public class RenderSurfaceMouseEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="RenderSurfaceMouseEventArgs"/>.
        /// </summary>
        /// <param name="x">The X position of the mouse when the event occured.</param>
        /// <param name="y">The Y position of the mouse when the event occured.</param>
        public RenderSurfaceMouseEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the X coordinate position of the mouse over a <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y coordinate position of the mouse over a <see cref="RenderSurfaceWindow"/>.
        /// </summary>
        public int Y { get; }
        #endregion
    }
}