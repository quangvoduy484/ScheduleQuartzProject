using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleQuartz.Services.FileProviderService
{
    /// <summary>
    /// A file provider abstraction
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Reads the contents of the file into a byte array
        /// </summary>
        /// <param name="filePath">The file for reading</param>
        /// <returns>A byte array containing the contents of the file</returns>
        byte[] ReadAllBytes(string filePath);

        /// <summary>
        /// Returns the creation date and time of the specified file or directory
        /// </summary>
        /// <param name="path">The file or directory for which to obtain creation date and time information</param>
        /// <returns>
        /// A System.DateTime structure set to the creation date and time for the specified file or directory. This value
        /// is expressed in local time
        /// </returns>
        DateTime GetCreationTime(string path);

        /// <summary>
        /// Returns the date and time the specified file or directory was last written to
        /// </summary>
        /// <param name="path">The file or directory for which to obtain write date and time information</param>
        /// <returns>
        /// A System.DateTime structure set to the date and time that the specified file or directory was last written to.
        /// This value is expressed in local time
        /// </returns>
        DateTime GetLastWriteTime(string path);

        /// <summary>
        /// Returns the date and time the specified file or directory was last accessed
        /// </summary>
        /// <param name="path">The file or directory for which to obtain access date and time information</param>
        /// <returns>A System.DateTime structure set to the date and time that the specified file</returns>
        DateTime GetLastAccessTime(string path);

        bool FileExists(string filePath);

        string Combine(params string[] paths);

        bool DirectoryExists(string path);

        void CreateDirectory(string path);

        void CreateFile(string path);

        // write file image , pdf ,...
        void WriteAllBytes(string filePath, byte[] bytes);

        // write file txt
        void WriteAllText(string path, string contents, Encoding encoding);

        string ReadAllTexts(string filePath, Encoding encoding);

        void DeleteFiles(string filePath);
    }
}
