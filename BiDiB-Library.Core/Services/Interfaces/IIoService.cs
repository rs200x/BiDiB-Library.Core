using System.Collections.Generic;
using System.IO;

namespace org.bidib.netbidibc.core.Services.Interfaces
{
    public interface IIoService
    {
        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="directoryPath">The directory path</param>
        /// <returns><c>true</c> if exists</returns>
        bool DirectoryExists(string directoryPath);

        /// <summary>
        /// Creates all directories in the specified path
        /// </summary>
        /// <param name="directoryPath">The directory path</param>
        void CreateDirectory(string directoryPath);

        /// <summary>
        /// Deletes the specified directory
        /// </summary>
        /// <param name="directoryPath"></param>
        void DeleteDirectory(string directoryPath);

        /// <summary>
        /// Determines whether the given file exists
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns><c>true</c> if exists</returns>
        bool FileExists(string filePath);

        /// <summary>
        /// Determines the file name of the file path
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>file name</returns>
        string GetFileName(string filePath);

        /// <summary>
        /// Creates a temporary folder
        /// </summary>
        /// <returns></returns>
        string GetTempFolder();

        /// <summary>
        /// Reads all lines from the specified file
        /// </summary>
        /// <param name="filePath">The full file path</param>
        /// <returns>file data lines</returns>
        IEnumerable<string> GetDataLines(string filePath);

        /// <summary>
        /// Reads all lines from the specified file contained within a zip source file
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <param name="sourceFile">The full source file path</param>
        /// <returns>file data lines</returns>
        IEnumerable<string> GetDataLines(string fileName, string sourceFile);

        /// <summary>
        /// Returns the names of subdirectories in the specified directory
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        string[] GetDirectories(string directoryPath);

        /// <summary>
        /// Returns the names of files in the specified directory
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        string[] GetFiles(string directoryPath, string filter);

        /// <summary>
        /// Extracts the content of zip file into the destination path 
        /// </summary>
        /// <param name="filePath">The zip file path</param>
        /// <param name="destinationPath">The destination directory path</param>
        void ExtractZip(string filePath, string destinationPath);

        /// <summary>
        /// Determines the directory path based on file path
        /// </summary>
        /// <param name="fileName">The file path</param>
        /// <returns>The directory path.</returns>
        string GetDirectory(string fileName);

        /// <summary>
        /// Determines the SHA1 value of the file
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>The SHA1 value.</returns>
        string GetSha1(string filePath);

        /// <summary>
        /// Gets the file stream from a file inside a zip archive
        /// </summary>
        /// <param name="archiveFileName">The archive file name</param>
        /// <param name="entryFileName">The archive entry file name</param>
        /// <returns></returns>
        Stream GetFileStreamFromArchive(string archiveFileName, string entryFileName);

        /// <summary>
        /// Determines the extension of the file
        /// </summary>
        /// <param name="fileName">The file path</param>
        /// <returns></returns>
        string GetFileExtension(string fileName);

        /// <summary>
        /// Writes the content to the specified file
        /// </summary>
        /// <param name="filePath">The target file path</param>
        /// <param name="fileContent">The file content</param>
        bool SaveToFile(string filePath, string fileContent);

        /// <summary>
        /// Stores the files into the specified file
        /// </summary>
        /// <param name="filePath">The target file path</param>
        /// <param name="files">The content files</param>
        void SaveToZip(string filePath, ICollection<string> files);


        string GetPath(params string[] paths);
    }
}