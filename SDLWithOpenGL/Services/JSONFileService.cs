using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SDLWithOpenGL.Services
{
    /// <summary>
    /// Creates JSON files with data.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class JSONFileService : IFileService
    {
        #region Private Fields
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
        #endregion


        #region Public Methods
        /// <summary>
        /// Saves the given <paramref name="data"/> to the given <paramref name="path"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to save into the file.</typeparam>
        /// <param name="path">The path of where to save the file.</param>
        /// <param name="data">The data to save in the file.</param>
        public void Save<T>(string path, T data) where T : class => File.WriteAllText(path, JsonSerializer.Serialize<T>(data, _jsonOptions));


        /// <summary>
        /// Loads a file at the given <paramref name="path"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to load from the file.</typeparam>
        /// <param name="path">The directory path to the file.</param>
        /// <returns>The data of type <typeparamref name="T"/>.</returns>
        public T Load<T>(string path) where T : class => JsonSerializer.Deserialize<T>(File.ReadAllText(path), _jsonOptions);


        /// <summary>
        /// Renames a file at the given <paramref name="path"/> to the given <paramref name="newName"/>.
        /// </summary>
        /// <param name="path">The path to the file to rename.</param>
        /// <param name="newName">The new name to give the file.  Any file extensions will be ignored.</param>
        /// <exception cref="ArgumentException">Thrown if the given <paramref name="path"/> has no extension.</exception>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Rename(string path, string newName)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path), "The param cannot be null");

            if (!Path.HasExtension(path))
                throw new ArgumentException($"The path must have a file name with an extension.", nameof(path));

            newName = Path.HasExtension(newName) ? Path.GetFileNameWithoutExtension(newName) : newName;

            var pathSections = path.Split('\\');
            var oldFileName = Path.GetFileName(path);

            File.Move(path, $@"{pathSections.Join(oldFileName)}\{newName}{Path.GetExtension(oldFileName)}");
        }


        /// <summary>
        /// Returns a value indicating if the file at the given <paramref name="path"/> exists.
        /// </summary>
        /// <param name="path">The path to the file to check for.</param>
        /// <returns></returns>
        public bool Exists(string path) => File.Exists(path);


        /// <summary>
        /// Deletes the file at the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        public void Delete(string path) => File.Delete(path);


        /// <summary>
        /// Copies the file at the given <paramref name="sourcePath"/> to the given <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="sourcePath">The source of the file to copy.</param>
        /// <param name="destinationPath">The destination of the file to copy.</param>
        /// <param name="overwrite">True if the destination file can be overwritten; otherwise, false.</param>
        public void Copy(string sourcePath, string destinationPath, bool overwrite = false) => File.Copy(sourcePath, destinationPath, overwrite);
        #endregion
    }
}
