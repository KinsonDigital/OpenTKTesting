using SDL2;
using System;
using System.Diagnostics.CodeAnalysis;
using static SDL2.SDL;

namespace SDLWithOpenGL.SDL2
{
    /// <summary>
    /// A thin wrapper around SDL method calls.
    /// </summary>
    public interface ISDLInvoker
    {
        /// <summary>
        /// Use this function to destroy the specified texture.
        /// </summary>
        /// <param name="texture">The texture to destroy.</param>
        void DestroyTexture(IntPtr texture);


        /// <summary>
        /// Use this function to destroy the rendering context for a window and free associated textures.
        /// </summary>
        /// <param name="renderer">The rendering context.</param>
        void DestroyRenderer(IntPtr renderer);


        /// <summary>
        /// Use this function to set the color used for drawing operations (Rect, Line and Clear).
        /// </summary>
        /// <param name="renderer">The rendering context.</param>
        /// <param name="r">The red value used to draw on the rendering target.</param>
        /// <param name="g">The green value used to draw on the rendering target.</param>
        /// <param name="b">The blue value used to draw on the rendering target.</param>
        /// <param name="a">the alpha value used to draw on the rendering target; usually SDL_ALPHA_OPAQUE (255). Use <see cref="SetRenderDrawBlendMode"/> to specify how the alpha channel is used</param>
        /// <returns>Returns 0 on success or a negative error code on failure; call <see cref="GetError"/>() for more information.</returns>
        int SetRenderDrawColor(IntPtr renderer, byte r, byte g, byte b, byte a);


        /// <summary>
        /// Use this function to retrieve a message about the last error that occurred.
        /// </summary>
        /// <returns>Returns a message with information about the specific error that occurred, or an empty string if there
        /// hasn't been an error message set since the last call to SDL_ClearError(). The message is only applicable when
        /// an SDL function has signaled an error. You must check the return values of SDL function calls to determine when
        /// to appropriately call <see cref="GetError"/>().</returns>
        string GetError();


        /// <summary>
        /// Use this function to create a 2D rendering context for a window.
        /// </summary>
        /// <param name="window">The window where rendering is displayed.</param>
        /// <param name="index">The index of the rendering driver to initialize, or -1 to initialize the first supporting the requested flags.</param>
        /// <param name="flags">0, or one or more <see cref="SDL_RendererFlags"/> OR'd together.</param>
        /// <returns>Returns a valid rendering context or NULL if there was an error; call SDL_GetError() for more information.</returns>
        IntPtr CreateRenderer(IntPtr window, int index, SDL_RendererFlags flags);


        /// <summary>
        /// Use this function to get the renderer associated with a window.
        /// </summary>
        /// <param name="window">The window to query.</param>
        /// <returns>Returns the rendering context on success or NULL on failure; call <see cref="GetError"/>() for more information.</returns>
        IntPtr GetRenderer(IntPtr window);


        /// <summary>
        /// Use this function to create a texture from an existing surface.
        /// </summary>
        /// <param name="renderer">The rendering context.</param>
        /// <param name="surface">The <see cref="SDL_Surface"/> structure containing pixel data used to fill the texture.</param>
        /// <returns>Returns the created texture or NULL on failure; call <see cref="GetError"/>() for more information.</returns>
        IntPtr CreateTextureFromSurface(IntPtr renderer, IntPtr surface);


        /// <summary>
        /// Use this function to query the attributes of a texture.
        /// </summary>
        /// <param name="texture">The texture to query.</param>
        /// <returns>Returns a tuple filled with information about a texture.
        /// Tuple Member result: The success or error code.  Value 0 on success or a negative error code on failure; call <see cref="GetError"/>() for more information.
        /// Tuple Member format: The raw format of the texture; the actual format may differ, but pixel transfers will use this format.
        /// Tuple Member access: The actual access to the texture (one of the SDL_TextureAccess values).
        /// Tuple Member w: The width of the texture in pixels.
        /// Tuple Member h: The height of the texture in pixels.</returns>
        (int result, uint format, int access, int w, int h) QueryTexture(IntPtr texture);


        /// <summary>
        /// Use this function to free an RGB surface.
        /// </summary>
        /// <param name="surface">The <see cref="SDL_Surface"/> to free.</param>
        void FreeSurface(IntPtr surface);


        /// <summary>
        /// Use this function to set the blend mode for a texture, used by RenderCopy().
        /// </summary>
        /// <param name="texture">The texture to update.</param>
        /// <param name="blendMode">The <see cref="SDL_BlendMode"/> to use for texture blending.</param>
        /// <returns>Returns 0 on success or a negative error code on failure; call <see cref="GetError"/>() for more information.</returns>
        int SetTextureBlendMode(IntPtr texture, SDL_BlendMode blendMode);


        /// <summary>
        /// Use this function to set an additional color value multiplied into render copy operations.
        /// </summary>
        /// <param name="texture">The texture to update.</param>
        /// <param name="r">The red color value multiplied into copy operations.</param>
        /// <param name="g">The green color value multiplied into copy operations.</param>
        /// <param name="b">The blue color value multiplied into copy operations.</param>
        /// <returns>Returns 0 on success or a negative error code on failure; call <see cref="GetError"/>() for more information.</returns>
        int SetTextureColorMod(IntPtr texture, byte r, byte g, byte b);


        /// <summary>
        /// Use this function to set an additional alpha value multiplied into render copy operations.
        /// </summary>
        /// <param name="texture">The texture to update.</param>
        /// <param name="alpha">The source alpha value multiplied into copy operations.</param>
        /// <returns>Returns 0 on success or a negative error code on failure; call <see cref="GetError"/>() for more information.</returns>
        int SetTextureAlphaMod(IntPtr texture, byte alpha);


        /// <summary>
        /// Use this function to copy a portion of the texture to the current rendering target, optionally rotating it by
        /// angle around the given center and also flipping it top-bottom and/or left-right.
        /// </summary>
        /// <param name="renderer">The rendering context.</param>
        /// <param name="texture">The source texture.</param>
        /// <param name="srcrect">The source <see cref="SDL_Rect"/> structure or NULL for the entire texture.</param>
        /// <param name="dstrect">The destination <see cref="SDL_Rect"/> structure or NULL for the entire rendering target.</param>
        /// <param name="angle">The angle in degrees that indicates the rotation that will be applied to dstrect, rotating it in a clockwise direction.</param>
        /// <param name="center">A pointer to a point indicating the point around which dstrect will be rotated (if NULL, rotation will be done around dstrect.w/2, dstrect.h/2).</param>
        /// <param name="flip">A <see cref="SDL_RendererFlip"/> value stating which flipping actions should be performed on the texture.</param>
        /// <returns>Returns 0 on success or a negative error code on failure; call <see cref="GetError"/>() for more information.</returns>
        int RenderCopyEx(IntPtr renderer, IntPtr texture, SDL_Rect srcrect, SDL_Rect dstrect, double angle, SDL_Point center, SDL_RendererFlip flip);


        /// <summary>
        /// Use this function to update the screen with any rendering performed since the previous call.
        /// </summary>
        /// <param name="renderer">The rendering context.</param>
        void RenderPresent(IntPtr renderer);


        /// <summary>
        /// Use this function to clear the current rendering target with the drawing color.
        /// </summary>
        /// <param name="renderer">The rendering context.</param>
        /// <returns>Returns 0 on success or a negative error code on failure; call <see cref="GetError"/>() for more information.</returns>
        int RenderClear(IntPtr renderer);


        /// <summary>
        /// Use this function to get the dots/pixels-per-inch for a display.
        /// </summary>
        /// <param name="displayIndex">The index of the display from which DPI information should be queried.</param>
        /// <returns>A tuple of result information of typle type (int, float, float, float).
        /// Tuple Member result: The success or a negative error code on failure; call <see cref="GetError"/>() for more information.
        /// Tuple Member ddpi: The diagonal DPI of the display.
        /// Tuple Member hdpi: The horizontal DPI of the display.
        /// Tuple Member vdpi: The verital DPI of the display.</returns>
        (int result, float ddpi, float hdpi, float vdpi) GetDisplayDPI(int displayIndex);


        /// <summary>
        /// Use this function to create a window with the specified position, dimensions, and flags.
        /// </summary>
        /// <param name="title">The title of the window, in UTF-8 encoding</param>
        /// <param name="x">The x position of the window, <see cref="SDL_WINDOWPOS_CENTERED"/>, or <see cref="SDL_WINDOWPOS_UNDEFINED"/>.</param>
        /// <param name="y">The y position of the window, <see cref="SDL_WINDOWPOS_CENTERED"/>, or <see cref="SDL_WINDOWPOS_UNDEFINED"/>.</param>
        /// <param name="w">The width of the window, in screen coordinates.</param>
        /// <param name="h">The height of the window, in screen coordinates.</param>
        /// <param name="flags">0, or one or more <see cref="SDL_WindowFlags"/> OR'd together.</param>
        /// <returns>Returns the window that was created or NULL on failure; call SDL_GetError() for more information.</returns>
        IntPtr CreateWindow(string title, int x, int y, int w, int h, SDL_WindowFlags flags);


        /// <summary>
        /// Use this function to get the position of a window.
        /// </summary>
        /// <param name="window">The window to query.</param>
        /// <returns>A tuple of (int, int).
        /// Tuple Member x: is the x position of the window, in screen coordinates.
        /// Tuple Member y: is the y position of the window, in screen coordinates.
        /// </returns>
        (int x, int y) GetWindowPosition(IntPtr window);


        /// <summary>
        /// Use this function to set the position of a window.
        /// </summary>
        /// <param name="window">The window to reposition.</param>
        /// <param name="x">The x position of the window, <see cref="SDL_WINDOWPOS_CENTERED"/>, or <see cref="SDL_WINDOWPOS_UNDEFINED"/>.</param>
        /// <param name="y">The y position of the window, <see cref="SDL_WINDOWPOS_CENTERED"/>, or <see cref="SDL_WINDOWPOS_UNDEFINED"/>.</param>
        void SetWindowPosition(IntPtr window, int x, int y);


        /// <summary>
        /// Use this function to get the size of a window's client area.
        /// </summary>
        /// <param name="window">The window to query the width and height from.</param>
        /// <returns>Returns the width and height of the window as a tuple of type (int, int).
        /// Tuple Member w: The width fo the window, in screen coordinates.
        /// Tuple Member h: The height fo the window, in screen coordinates.</returns>
        (int w, int h) GetWindowSize(IntPtr window);


        /// <summary>
        /// Use this function to set the size of a window's client area.
        /// </summary>
        /// <param name="window">The window to change.</param>
        /// <param name="w">The width of the window in pixels, in screen coordinates, must be > 0.</param>
        /// <param name="h">The height of the window in pixels, in screen coordinates, must be > 0.</param>
        void SetWindowSize(IntPtr window, int w, int h);


        /// <summary>
        /// Use this function to get the window flags.
        /// </summary>
        /// <param name="window">The window to query.</param>
        /// <returns>Returns a mask of the <see cref="SDL_WindowFlags"/> associated with window.</returns>
        uint GetWindowFlags(IntPtr window);


        /// <summary>
        /// Use this function to set the border state of a window.
        /// </summary>
        /// <param name="window">The window of which to change the border state.</param>
        /// <param name="bordered"><see cref="SDL_bool.SDL_FALSE"/> to remove border, <see cref="SDL_bool.SDL_TRUE"/> to add border.</param>
        void SetWindowBordered(IntPtr window, SDL_bool bordered);


        /// <summary>
        /// Use this function to set the user-resizable state of a window.
        /// </summary>
        /// <param name="window">The window of which to change the resizable state.</param>
        /// <param name="resizable"><see cref="SDL_bool.SDL_TRUE"/> to allow resizing, <see cref="SDL_bool.SDL_FALSE"/> to disallow.</param>
        void SetWindowResizable(IntPtr window, SDL_bool resizable);


        /// <summary>
        /// Use this function to show a window.
        /// </summary>
        /// <param name="window">The window to show.</param>
        void ShowWindow(IntPtr window);


        /// <summary>
        /// Use this function to hide a window.
        /// </summary>
        /// <param name="window">The window to hide.</param>
        void HideWindow(IntPtr window);


        /// <summary>
        /// Use this function to make a window as large as possible.
        /// </summary>
        /// <param name="window">The window to maximize.</param>
        void MaximizeWindow(IntPtr window);


        /// <summary>
        /// Use this function to minimize a window to an iconic representation.
        /// </summary>
        /// <param name="window">The window to minimize.</param>
        void MinimizeWindow(IntPtr window);


        /// <summary>
        /// Use this function to restore the size and position of a minimized or maximized window.
        /// </summary>
        /// <param name="window">The window to restore.</param>
        void RestoreWindow(IntPtr window);


        /// <summary>
        /// Use this function to raise a window above other windows and set the input focus.
        /// </summary>
        /// <param name="window">The window to raise.</param>
        [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "<Pending>")]
        void RaiseWindow(IntPtr window);


        /// <summary>
        /// Use this function to destroy a window.
        /// </summary>
        /// <param name="window">The window to destroy.</param>
        void DestroyWindow(IntPtr window);


        /// <summary>
        /// Use this function to get driver specific information about a window.
        /// </summary>
        /// <param name="window">The window about which information is being requested.</param>
        /// <param name="info">An <see cref="SDL_SysWMinfo"/> structure filled in with window information.</param>
        /// <returns> Returns information related to getting window related information.
        /// Tuple Member success: <see cref="SDL_bool.SDL_TRUE"/> if the function is implemented and the version member of the info struct is valid,
        /// or <see cref="SDL_bool.SDL_FALSE"/> if the information could not be retrieved; call <see cref="GetError"/>() for more information.</returns>
        /// Tuple Member info: The window related information being requested.
        (SDL_bool success, SDL_SysWMinfo info) GetWindowWMInfo(IntPtr window);


        /// <summary>
        /// Use this function to initialize the SDL library. This must be called before using most other SDL functions.
        /// </summary>
        /// <param name="flags">Subsystem initialization flags.</param>
        /// <returns>Returns 0 on success or a negative error code on failure; call <see cref="GetError"/>() for more information.</returns>
        int Init(uint flags);


        /// <summary>
        /// Use this function to set a hint with normal priority.
        /// </summary>
        /// <param name="name">The int to set.</param>
        /// <param name="value">The value of the int variable.</param>
        /// <returns>Returns <see cref="SDL_bool.SDL_TRUE"/> if the hint was set, <see cref="SDL_bool.SDL_FALSE"/> otherwise.</returns>
        SDL_bool SetHint(string name, string value);


        /// <summary>
        /// Use this function to poll for curently pending events.
        /// </summary>
        /// <returns>Returns a tuple with the event results.
        /// Tuple Param haspendingEvents: True if there are events that need to be processed.
        /// Tuple Param sdlEvent: The event related information.
        /// </returns>
        public (bool hasPendingEvents, SDL_Event sdlEvent) PollEvent();


        /// <summary>
        /// Returns a value indicating if the given image format has been loaded.
        /// </summary>
        /// <param name="imgFormat">The image format to check.</param>
        /// <returns>True if the image format has already been initialized.</returns>
        //TODO: This needs some manually testing
        public bool IsImgInit(SDL_image.IMG_InitFlags imgFormat);


        /// <summary>
        /// Loads the file for us as an image in a new surface.
        /// </summary>
        /// <param name="path">Image file name to load a surface from.</param>
        /// <returns>Returns a pointer to the image.  A pointer value of zero means something went wrong during the loading process.</returns>
        IntPtr ImgLoad(string path);


        /// <summary>
        /// Initialize by loading support for JPG images or at least return success if support is already
        /// loaded. You may call this multiple times, which will actually require you to call IMG_Quit just once to clean up.
        /// </summary>
        void ImgInitJPG();


        /// <summary>
        /// Initialize by loading support for PNG images or at least return success if support is already
        /// loaded. You may call this multiple times, which will actually require you to call IMG_Quit just once to clean up.
        /// </summary>
        void ImgInitPNG();


        /// <summary>
        /// Initialize by loading support for TIF images or at least return success if support is already
        /// loaded. You may call this multiple times, which will actually require you to call IMG_Quit just once to clean up.
        /// </summary>
        void ImgInitTIF();


        /// <summary>
        /// Initialize by loading support for WEBP images or at least return success if support is already
        /// loaded. You may call this multiple times, which will actually require you to call IMG_Quit just once to clean up.
        /// </summary>
        void ImgInitWEBP();


        /// <summary>
        /// This function cleans up all dynamically loaded library handles, freeing memory. If support is required again it
        /// will be initialized again, either by <see cref="Init"/> or loading an image with dynamic support required. You may call
        /// this function when IMG_Load functions are no longer needed for the JPG, PNG, and TIF image formats. You only need
        /// to call this function once, no matter how many times <see cref="Init"/> was called.
        /// </summary>
        void ImageQuit();


        /// <summary>
        /// Use this function to clean up all initialized subsystems. You should call it upon all exit conditions.
        /// </summary>
        void Quit();
    }
}