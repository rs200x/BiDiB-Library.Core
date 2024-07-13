using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Models.Xml;
using org.bidib.Net.Core.Properties;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core.Services;

[Export(typeof(IXmlService))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class XmlService : IXmlService
{
    private readonly IIoService ioService;
    private readonly ILogger<XmlService> logger;
    private readonly ILogger exceptionLogger;

    [ImportingConstructor]
    public XmlService(IIoService ioService, ILoggerFactory loggerFactory)
    {
        this.ioService = ioService;
        logger = loggerFactory.CreateLogger<XmlService>();
        exceptionLogger = loggerFactory.CreateLogger(BiDiBConstants.LoggerContextException);
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
    }

    public IXmlValidationInfo ValidateFile(string filePath, string schema, string xsdFilePath)
    {
        if (string.IsNullOrEmpty(filePath) || !ioService.FileExists(filePath)) { return GetInvalidFileInfo(filePath); }
        if (string.IsNullOrEmpty(schema)) { return new XmlValidationInfo(XmlValidationResult.NoSchema, Resources.SchemaMustNotBeNull); }
        if (string.IsNullOrEmpty(xsdFilePath) || !ioService.FileExists(xsdFilePath)) { return GetInvalidFileInfo(xsdFilePath); }

        StringBuilder errorString = new();
        try
        {
            var xmlDocument = new XmlDocument { XmlResolver = null };
            xmlDocument.Schemas.Add(schema, xsdFilePath);
            xmlDocument.Load(filePath);

            if (xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.NamespaceURI == schema)
            {
                xmlDocument.Validate((_, args) =>
                {
                    errorString.AppendLine(string.Format(CultureInfo.CurrentUICulture, "{0}: {1}", args.Severity, args.Message));
                });
            }
            else
            {
                errorString.AppendLine(string.Format(CultureInfo.CurrentUICulture, Resources.FileHasNoDefinedSchema, filePath, schema));
                logger.LogError("Schema validation error:" + errorString);
                return new XmlValidationInfo(XmlValidationResult.NoSchema, errorString.ToString());
            }
        }
        catch (XmlSchemaValidationException ex)
        {
            errorString.AppendLine(string.Format(CultureInfo.CurrentUICulture, Resources.FileNotValidWithSchema, filePath, xsdFilePath));
            logger.LogError(errorString.ToString());
            exceptionLogger.LogError(ex, "Schema validation error");
            return new XmlValidationInfo(XmlValidationResult.ValidationError, errorString.ToString());
        }
        catch (Exception ex)
        {
            errorString.AppendLine(string.Format(CultureInfo.CurrentUICulture, Resources.XmlValidationProcessError, filePath, schema, xsdFilePath));
            logger.LogError(errorString.ToString());
            exceptionLogger.LogError(ex, "xml validation error");
            return new XmlValidationInfo(XmlValidationResult.ValidationError, errorString.ToString());
        }

        var errorMessage = errorString.ToString();

        if (string.IsNullOrEmpty(errorMessage)) { return new XmlValidationInfo(XmlValidationResult.Valid, null); }

        logger.LogError(string.Format(CultureInfo.CurrentUICulture, Resources.FileNotValidWithSchema, filePath, xsdFilePath));
        logger.LogError(errorMessage);
        return new XmlValidationInfo(XmlValidationResult.ValidationError, errorMessage);
    }

    public T LoadFromFile<T>(string filePath) where T : class
    {
        T data = null;

        try
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var reader = XmlReader.Create(filePath);
            data = xmlSerializer.Deserialize(reader) as T;
        }
        catch (Exception ex) when (ex is FileNotFoundException or UriFormatException)
        {
            var error = $"File '{filePath}' could not be loaded for deserialization'";
            logger.LogError(ex, error);
        }
        catch (Exception ex) when (ex is ArgumentNullException or SecurityException or InvalidOperationException)
        {
            var error = $"Content of '{filePath}' could not be deserialized into object of type '{typeof(T)}'";
            logger.LogError(ex, error);
            exceptionLogger.LogError(ex, error);
        }

        return data;
    }

    /// <inheritdoc />
    public T LoadFromStream<T>(Stream dataStream) where T : class
    {
        T data = null;

        try
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var reader = XmlReader.Create(dataStream);
            data = xmlSerializer.Deserialize(reader) as T;
        }
        catch (Exception ex) when (ex is ArgumentNullException or SecurityException or InvalidOperationException)
        {
            logger.LogError(ex, $"Content of stream could not be deserialized into object of type '{typeof(T)}'");
        }

        return data;
    }

    public bool SaveToFile<T>(T instance, string filePath, ICollection<XmlNamespace> requiredNamespaces = null, bool prettyPrintAttributes = false) where T : class
    {
        try
        {
            var settings = new XmlWriterSettings { Indent = true, NewLineOnAttributes = prettyPrintAttributes };
            var namespaces = new XmlSerializerNamespaces();
            if (requiredNamespaces != null)
            {
                foreach (var requiredNamespace in requiredNamespaces)
                {
                    namespaces.Add(requiredNamespace.Prefix, requiredNamespace.Url);
                }
            }

            var xmlSerializer = new XmlSerializer(instance.GetType());
            using var writer = XmlWriter.Create(filePath, settings);
            xmlSerializer.Serialize(writer, instance, namespaces);
        }
        catch (Exception ex)
        {
            logger.LogError($"Instance of type '{instance.GetType()}' could not be serialized to file '{filePath}'");
            exceptionLogger.LogError(ex, "serialization error");
            return false;
        }

        return true;
    }

    public bool FileHasNamespace(string filePath, string namespaceUri)
    {
        if (string.IsNullOrEmpty(filePath) || !ioService.FileExists(filePath)) { return false; }

        var xmlDocument = new XmlDocument { XmlResolver = null };
        xmlDocument.Load(filePath);

        return xmlDocument.DocumentElement == null && string.IsNullOrEmpty(namespaceUri)
               || xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.NamespaceURI == namespaceUri;
    }

    public bool TransformFile(string filePath, string transformationFilePath, string destinationFilePath = null)
    {
        if (!ioService.FileExists(filePath)) { return false; }
        if (!ioService.FileExists(transformationFilePath)) { return false; }
        try
        {
            // Load the style sheet.
            var xslt = new XslCompiledTransform();

            xslt.Load(transformationFilePath);

            var argumentList = new XsltArgumentList();
            argumentList.AddExtensionObject("urn:MoExt", new XslTransformationExtensions());

            var useTempFile = false;
            if (string.IsNullOrEmpty(destinationFilePath) || filePath == destinationFilePath)
            {
                destinationFilePath = Path.GetRandomFileName();
                useTempFile = true;
            }

            // Execute the transform and output the results to a file.
            using (var writer = XmlWriter.Create(destinationFilePath))
            {
                xslt.Transform(filePath, argumentList, writer);
            }

            if (!useTempFile) { return true; }

            File.Copy(destinationFilePath, filePath, true);
            File.Delete(destinationFilePath);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Migration of '{filePath}' failed!");
            return false;
        }
    }

    public void SaveToArchive(string filePath, string fileName, XmlDocument xml)
    {
        if (!ioService.FileExists(filePath)) { return; }
        using var archive = ZipFile.Open(filePath, ZipArchiveMode.Update);
        var entry = archive.Entries.FirstOrDefault(x => x.Name == fileName);

        if (entry == null)
        {
            return;
        }

        using var entryStream = entry.Open();
        xml.Save(entryStream);
        entryStream.Flush();
    }

    private static IXmlValidationInfo GetInvalidFileInfo(string fileName)
    {
        return new XmlValidationInfo(XmlValidationResult.FileError, string.Format(CultureInfo.CurrentUICulture, Resources.XmlValidationFileError, fileName));
    }
}