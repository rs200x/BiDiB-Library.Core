using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Security;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using org.bidib.Net.Core.Services.Interfaces;
using Formatting = Newtonsoft.Json.Formatting;

namespace org.bidib.Net.Core.Services;

[Export(typeof(IJsonService))]
[PartCreationPolicy(CreationPolicy.NonShared)]
[method: ImportingConstructor]
public class JsonService(ILoggerFactory loggerFactory) : IJsonService
{
    private readonly ILogger<JsonService> logger = loggerFactory.CreateLogger<JsonService>();
    private readonly ILogger exceptionLogger = loggerFactory.CreateLogger(BiDiBConstants.LoggerContextException);

    private readonly JsonSerializerSettings serializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
        DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff",
        ContractResolver = new CamelCasePropertyNamesContractResolver()
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

    /// <inheritdoc />
    public bool SaveToFile(object data, string filePath)
    {
        return SaveToFile(data, filePath, false);
    }


    /// <inheritdoc />
    public bool SaveToFile(object data, string filePath, bool compressed)
    {
        try
        {
            serializerSettings.Formatting = compressed ? Formatting.None : Formatting.Indented;
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