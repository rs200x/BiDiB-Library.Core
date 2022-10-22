using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Services.Interfaces;

namespace org.bidib.netbidibc.core.Services
{
    public class IoService : IIoService
    {
        private readonly ILogger<IoService> logger;

        public IoService(ILogger<IoService> logger)
        {
            this.logger = logger;
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
        public List<string> GetDataLines(string filePath)
        {
            List<string> dataLines = new List<string>();

            if (!FileExists(filePath)) return dataLines;

            using StreamReader stream = new StreamReader(filePath);
            while (!stream.EndOfStream)
            {
                dataLines.Add(stream.ReadLine());
            }

            return dataLines;
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path, string filter)
        {
            return !DirectoryExists(path) ? Array.Empty<string>() : Directory.GetFiles(path, filter);
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
    }
}