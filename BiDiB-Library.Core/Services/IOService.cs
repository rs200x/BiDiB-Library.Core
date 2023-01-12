using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Properties;
using org.bidib.netbidibc.core.Services.Interfaces;

namespace org.bidib.netbidibc.core.Services
{
    [Export(typeof(IIoService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class IoService : IIoService
    {
        private readonly ILogger<IoService> logger;

        [ImportingConstructor]
        public IoService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<IoService>();
        }

        public bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public void CreateDirectory(string directoryPath)
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
                logger.LogDebug($"Directory created: '{directoryPath}'");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to create directory '{directoryPath}'!");
            }
        }

        /// <inheritdoc />
        public void DeleteDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath)) { return; }
            Directory.Delete(directoryPath, true);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <inheritdoc />
        public string GetFileName(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) ? new FileInfo(filePath).Name : string.Empty;
        }

        public string GetTempFolder()
        {
            string tempPath = Path.GetTempPath();
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(tempPath, Guid.NewGuid().ToString()));
            return di.FullName;
        }

        /// <inheritdoc />
        public string GetPath(params string[] paths)
        {
            if (paths == null || paths.Length == 0) { return string.Empty; }

            return Path.Combine(paths);
        }

        /// <inheritdoc />
        public string GetDirectory(string fileName)
        {
            if (!FileExists(fileName)) { return string.Empty; }
            FileInfo fileInfo = new FileInfo(fileName);
            return fileInfo.DirectoryName;
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path, string filter)
        {
            return !DirectoryExists(path) ? Array.Empty<string>() : Directory.GetFiles(path, filter);
        }

        /// <inheritdoc />
        public bool SaveToFile(string filePath, string fileContent)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileContent))
            {
                logger.LogError(Resources.Error_FilePathContentEmpty);
                return false;
            }

            try
            {
                File.WriteAllText(filePath, fileContent);
                return true;
            }
            catch (Exception e) when (e is PathTooLongException or DirectoryNotFoundException or IOException)
            {
                logger.LogError(string.Format(CultureInfo.InvariantCulture, Resources.Error_FileSaveException, filePath), e);
            }
            catch (Exception e) when (e is UnauthorizedAccessException or SecurityException or NotSupportedException)
            {
               logger.LogError(string.Format(CultureInfo.InvariantCulture, Resources.Error_FileSaveException, filePath), e);
            }

            return false;
        }

        public void ExtractZip(string filePath, string destinationPath)
        {
            if (!FileExists(filePath)) { return; }
            if (!DirectoryExists(destinationPath)) { return; }

            ZipFile.ExtractToDirectory(filePath, destinationPath);
        }

        public string GetSha1(string filePath)
        {
            if (!FileExists(filePath))
            {
                return null;
            }

            var fileInfo = new FileInfo(filePath);
            byte[] hashValue;
            using (FileStream fileStream = File.OpenRead(fileInfo.FullName))
            using (var provider = SHA1.Create())
            {
                fileStream.Position = 0;
                hashValue = provider.ComputeHash(fileStream);
            }

            return string.Join("", hashValue.Select(x => $"{x:x2}"));

        }

        /// <inheritdoc />
        public IEnumerable<string> GetDataLines(string filePath)
        {
            List<string> dataLines = new List<string>();

            if (!FileExists(filePath)) { return dataLines; }

            using StreamReader stream = new StreamReader(filePath);
            dataLines.AddRange(GetLinesFromStream(stream));

            return dataLines;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDataLines(string fileName, string sourceFile)
        {
            List<string> dataLines = new List<string>();

            if (!FileExists(sourceFile) || new FileInfo(sourceFile).Extension.ToUpperInvariant() != ".ZIP") { return dataLines; }

            var archive = ZipFile.OpenRead(sourceFile);
            
            var fileEntry = archive.Entries.FirstOrDefault(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            if (fileEntry == null)
            {
                return dataLines;
            }

            using (var stream = fileEntry.Open())
            {
                using (var reader = new StreamReader(stream))
                {
                    dataLines.AddRange(GetLinesFromStream(reader));
                }
            }

            archive.Dispose();
            return dataLines;
        }

        private static IEnumerable<string> GetLinesFromStream(StreamReader stream)
        {
            List<string> dataLines = new List<string>();
            if (stream == null)
            {
                return dataLines;
            }

            while (!stream.EndOfStream)
            {
                dataLines.Add(stream.ReadLine());
            }

            return dataLines;
        }

        /// <inheritdoc />
        public Stream GetFileStreamFromArchive(string archiveFileName, string entryFileName)
        {
            if (!FileExists(archiveFileName))
            {
                return null;
            }

            var archive = ZipFile.OpenRead(archiveFileName);
            var entry = archive.Entries.FirstOrDefault(x => x.Name == entryFileName);

            return entry?.Open();
        }

        /// <inheritdoc />
        public string GetFileExtension(string fileName)
        {
            return !FileExists(fileName) ? null : new FileInfo(fileName).Extension;
        }

        /// <inheritdoc />
        public void SaveToZip(string filePath, ICollection<string> files)
        {
            if (string.IsNullOrEmpty(filePath)) { return; }
            if (files == null || !files.Any()) { return; }

            var zip = ZipFile.Open(filePath, ZipArchiveMode.Create);
            foreach (var file in files)
            {
                zip.CreateEntryFromFile(file, GetFileName(file), CompressionLevel.Optimal);
            }
            zip.Dispose();
        }
    }
}