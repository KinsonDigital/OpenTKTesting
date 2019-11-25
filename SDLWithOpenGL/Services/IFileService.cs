﻿namespace SDLWithOpenGL.Services
{
    /// <summary>
    /// Creates JSON files with data.
    /// </summary>
    public interface IFileService
    {
        #region Methods
        /// <summary>
        /// Saves the given <paramref name="data"/> to the given <paramref name="path"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to save into the file.</typeparam>
        /// <param name="path">The path of where to save the file.</param>
        /// <param name="data">The data to save in the file.</param>
        void Save<T>(string path, T data) where T : class;


        /// <summary>
        /// Loads a file at the given <paramref name="path"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to load from the file.</typeparam>
        /// <param name="path">The directory path to the file.</param>
        /// <returns>The data of type <typeparamref name="T"/>.</returns>
        T Load<T>(string path) where T : class;


        /// <summary>
        /// Renames a file at the given <paramref name="path"/> to the given <paramref name="newName"/>.
        /// </summary>
        /// <param name="path">The path to the file to rename.</param>
        /// <param name="newName">The new name to give the file.</param>
        void Rename(string path, string newName);


        /// <summary>
        /// Returns a value indicating if the file at the given <paramref name="path"/> exists.
        /// </summary>
        /// <param name="path">The path to the file to check for.</param>
        /// <returns></returns>
        bool Exists(string path);


        /// <summary>
        /// Deletes the file at the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the file to delete.</param>
        void Delete(string path);


        /// <summary>
        /// Copies the file at the given <paramref name="sourcePath"/> to the given <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="sourcePath">The source of the file to copy.</param>
        /// <param name="destinationPath">The destination of the file to copy.</param>
        /// <param name="overwrite">True if the destination file can be overwritten; otherwise, false.</param>
        void Copy(string sourcePath, string destinationPath, bool overwrite);
        #endregion
    }
}