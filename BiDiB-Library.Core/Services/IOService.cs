using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Properties;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core.Services;

[Export(typeof(IIoService))]
[PartCreationPolicy(CreationPolicy.NonShared)]
[method: ImportingConstructor]
public class IoService(ILogger<IoService> logger) : IIoService
{
    /// <inheritdoc />
    public bool DirectoryExists(string directoryPath)
    {
        return Directory.Exists(directoryPath);
    }

    /// <inheritdoc />
    public void CreateDirectory(string directoryPath)
    {
        try
        {
            Directory.CreateDirectory(directoryPath);
            logger.LogDebug("Directory created: '{DirectoryPath}'", directoryPath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create directory '{DirectoryPath}'!", directoryPath);
        }
    }

    /// <inheritdoc />
    public void DeleteDirectory(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath)) { return; }
        Directory.Delete(directoryPath, true);
    }

    /// <inheritdoc />
    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <inheritdoc />
    public string GetFileName(string filePath)
    {
        return !string.IsNullOrEmpty(filePath) ? new FileInfo(filePath).Name : string.Empty;
    }

    /// <inheritdoc />
    public string GetTempFolder()
    {
        var randomDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var di = Directory.CreateDirectory(randomDir);
        return di.FullName;
    }

    /// <inheritdoc />
    public string GetPath(params string[] paths)
    {
        if (paths == null || paths.Length == 0) { return string.Empty; }

        return Path.Combine(paths);
    }

    /// <inheritdoc />
    public bool TryDelete(string filePath)
    {
        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (Exception e)
        {
            logger.LogDebug("File could not be deleted: {File} {Error}", GetFileName(filePath), e.Message);
        }

        return false;
    }

    /// <inheritdoc />
    public string GetDirectory(string fileName)
    {
        if (!FileExists(fileName)) { return string.Empty; }
        var fileInfo = new FileInfo(fileName);
        return fileInfo.DirectoryName;
    }

    /// <inheritdoc />
    public string[] GetDirectories(string directoryPath)
    {
        return Directory.GetDirectories(directoryPath);
    }

    /// <inheritdoc />
    public string[] GetFiles(string directoryPath, string filter)
    {
        return !DirectoryExists(directoryPath) ? Array.Empty<string>() : Directory.GetFiles(directoryPath, filter);
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
            logger.LogError(e, "Content could not be exported to '{FilePath}'", filePath);
        }
        catch (Exception e) when (e is UnauthorizedAccessException or SecurityException or NotSupportedException)
        {
            logger.LogError(e, "Content could not be exported to '{FilePath}' due to restrictions", filePath);
        }

        return false;
    }

    /// <inheritdoc />
    public void ExtractZip(string filePath, string destinationPath)
    {
        if (!FileExists(filePath)) { return; }
        if (!DirectoryExists(destinationPath)) { return; }

        ZipFile.ExtractToDirectory(filePath, destinationPath);
    }

    /// <inheritdoc />
    public string GetSha1(string filePath)
    {
        if (!FileExists(filePath))
        {
            return null;
        }

        var fileInfo = new FileInfo(filePath);
        byte[] hashValue;
        using (var fileStream = File.OpenRead(fileInfo.FullName))
        using (var provider = SHA1.Create())
        {
            fileStream.Position = 0;
            hashValue = provider.ComputeHash(fileStream);
        }

        return string.Join("", hashValue.Select(x => $"{x:x2}"));

    }

    /// <inheritdoc />
    public string GetData(string filePath)
    {
        return !FileExists(filePath) ? null : File.ReadAllText(filePath);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetDataLines(string filePath)
    {
        var dataLines = new List<string>();

        if (!FileExists(filePath)) { return dataLines; }

        using var stream = new StreamReader(filePath);
        dataLines.AddRange(GetLinesFromStream(stream));

        return dataLines;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetDataLines(string fileName, string sourceFile)
    {
        var dataLines = new List<string>();

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

    /// <inheritdoc />
    public string GetDataFromArchive(string filePath, string fileName)
    {
        var stream = GetFileStreamFromArchive(filePath, fileName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <inheritdoc />
    public Stream GetFileStreamFromArchive(string archiveFileName, string entryFileName)
    {
        if (!FileExists(archiveFileName))
        {
            return null;
        }

        using var archive = ZipFile.OpenRead(archiveFileName);
        var entry = archive.Entries.FirstOrDefault(x => x.Name == entryFileName);

        if (entry == null)
        {
            return null; 
        }

        var dataStream = entry.Open();
        MemoryStream readerStream = new MemoryStream();
        dataStream.CopyTo(readerStream);
        readerStream.Position = 0;

        return readerStream;
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

        using var zip = ZipFile.Open(filePath, ZipArchiveMode.Create);
        var tempFolder = GetTempFolder();
        foreach (var file in files)
        {
            try
            {
                var tempFile =GetPath(tempFolder, Path.GetRandomFileName());
                File.Copy(file, tempFile);
                zip.CreateEntryFromFile(tempFile, GetFileName(file), CompressionLevel.Optimal);
                File.Delete(tempFile);
            }
            catch (Exception e) when (e is UnauthorizedAccessException or SecurityException or NotSupportedException)
            {
                logger.LogError(e, "File '{File}' could not be added to archive '{Archive}'", file, filePath);
            }
        }
        DeleteDirectory(tempFolder);
    }

    /// <inheritdoc />
    public void SaveToZip(string filePath, string fileName, string fileContent)
    {
        if (string.IsNullOrEmpty(filePath) 
            || !FileExists(filePath) 
            || string.IsNullOrEmpty(fileName) 
            || string.IsNullOrEmpty(fileContent)) { return; }

        using var archive = ZipFile.Open(filePath, ZipArchiveMode.Update);

        var entry = archive.Entries.FirstOrDefault(x => x.Name == fileName);

        entry?.Delete();

        entry = archive.CreateEntry(fileName);

        using StreamWriter writer = new StreamWriter(entry.Open());
        writer.Write(fileContent);
    }

    private static IEnumerable<string> GetLinesFromStream(StreamReader stream)
    {
        var dataLines = new List<string>();
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
}