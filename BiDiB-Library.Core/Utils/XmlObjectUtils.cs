using System.IO;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Utils
{
    public static class XmlObjectUtils
    {
        public static T Clone<T>(this T item) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            return serializer.Deserialize(stream) as T;
        }
    }
}