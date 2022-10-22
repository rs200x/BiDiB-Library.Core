using System.IO;

namespace org.bidib.netbidibc.core.Services.Interfaces
{
    public interface IJsonService
    {
        /// <summary>
        /// Loads the content from the specified file and transforms it into the target type.
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="filePath">The content file path</param>
        /// <returns>Deserialized object from file content</returns>
        T LoadFromFile<T>(string filePath) where T : class;

        /// <summary>
        /// Deserializes date from the given stream into the specified object type
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="dataStream">The source stream</param>
        /// <returns></returns>
        T LoadFromStream<T>(Stream dataStream) where T : class;

        /// <summary>
        /// Persists the object to the specified file.
        /// </summary>
        /// <param name="data">The object to be persisted</param>
        /// <param name="filePath">The target file path</param>
        /// <returns></returns>
        bool SaveToFile(object data, string filePath);
    }
}