using System.IO;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Utils;

public static class XmlObjectUtils
{
    public static T Clone<T>(this T item) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        var stream = new MemoryStream();
        serializer.Serialize(stream, item);
        stream.Seek(0, SeekOrigin.Begin);
        return serializer.Deserialize(stream) as T;
    }
}