using System.Collections.Generic;
using System.IO;
using System.Xml;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Services.Interfaces;

public interface IXmlService
{
    IXmlValidationInfo ValidateFile(string filePath, string schema, string xsdFilePath);

    T LoadFromFile<T>(string filePath) where T : class;

    /// <summary>
    /// Deserializes date from the given stream into the specified object type
    /// </summary>
    /// <typeparam name="T">The object type</typeparam>
    /// <param name="dataStream">The source stream</param>
    /// <returns></returns>
    T LoadFromStream<T>(Stream dataStream) where T : class;

    bool SaveToFile<T>(T instance, string filePath, ICollection<XmlNamespace> requiredNamespaces = null, bool prettyPrintAttributes = false) where T : class;

    void SaveToArchive(string filePath, string fileName, XmlDocument xml);

    /// <summary>
    /// Determines if xml file is specified with given namespace
    /// </summary>
    /// <param name="filePath">The xml file path</param>
    /// <param name="namespaceUri">The namespace uri</param>
    /// <returns><c>true</c> if namespace match (even if no namespace and uri is empty)</returns>
    bool FileHasNamespace(string filePath, string namespaceUri);

    bool TransformFile(string filePath, string transformationFilePath, string destinationFilePath = null);
}