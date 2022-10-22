using System;
using System.IO;
using System.Security;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using org.bidib.netbidibc.core.Services.Interfaces;
using Formatting = Newtonsoft.Json.Formatting;

namespace org.bidib.netbidibc.core.Services
{

    public class JsonService : IJsonService
    {
        private readonly ILogger<JsonService> logger;
        private readonly ILogger exceptionLogger;

        public JsonService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<JsonService>();
            exceptionLogger = loggerFactory.CreateLogger("EXCEPTION");
        }

        private readonly JsonSerializerSettings serializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff"
        };

        public T LoadFromFile<T>(string filePath) where T : class
        {
            T data = null;

            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var content = File.ReadAllText(filePath);
                data = JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex) when (ex is FileNotFoundException or UriFormatException)
            {
                HandleError($"File '{filePath}' could not be loaded for deserialization'",ex);
            }
            catch (Exception ex) when (ex is ArgumentNullException or SecurityException or InvalidOperationException)
            {
                HandleError($"Content of '{filePath}' could not be deserialized into object of type '{typeof(T)}'", ex);
            }

            return data;
        }

        public T LoadFromStream<T>(Stream dataStream) where T : class
        {
            T data = null;

            try
            {
                using var r = new StreamReader(dataStream);
                data = JsonConvert.DeserializeObject<T>(r.ReadToEnd());
            }
            catch (Exception ex) when (ex is ArgumentNullException or SecurityException or InvalidOperationException)
            {
                HandleError($"Content of stream could not be deserialized into object of type '{typeof(T)}'",ex);
            }

            return data;
        }

        public bool SaveToFile(object data, string filePath)
        {
            try
            {
                var content = JsonConvert.SerializeObject(data, serializerSettings);
                File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception e)
            {
                HandleError($"netBiDiB participants could not be stored to: {filePath}", e);
            }

            return false;
        }

        private void HandleError(string error, Exception exception)
        {
            logger.LogError(exception, error);
            exceptionLogger.LogError(exception, error);
        }
    }
}