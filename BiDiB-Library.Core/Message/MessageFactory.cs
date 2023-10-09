using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Models.Messages;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Message;

[Export(typeof(IMessageFactory))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class MessageFactory : IMessageFactory
{
    private readonly ILogger<MessageFactory> logger;
    private static readonly IDictionary<BiDiBMessage, Type> MessageTypeMappings = new Dictionary<BiDiBMessage, Type>();

    [ImportingConstructor]
    public MessageFactory(ILogger<MessageFactory> logger)
    {
        this.logger = logger;
        GetTypeMappings();
    }

    /// <summary>
    /// Creates specific input message object of provided message
    /// </summary>
    /// <param name="messageBytes">the message bytes</param>
    /// <returns>the message object, null on any error</returns>
    public BiDiBInputMessage CreateInputMessage(byte[] messageBytes)
    {
        BiDiBInputMessage inputMessage;

        try
        {
            inputMessage = new BiDiBInputMessage(messageBytes);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Message '{MessageData}' could not be converted into generic input message", messageBytes.GetDataString());
            return null;
        }

        try
        {
            inputMessage = GetSpecific(inputMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Message '{MessageData}' could not be converted into specific input message", messageBytes.GetDataString());
        }

        return inputMessage;
    }

    private BiDiBInputMessage GetSpecific(BiDiBInputMessage inputMessage)
    {

        var classType = GetInputMessageType(inputMessage.MessageType);
        if (classType == null)
        {
            logger.LogDebug("There is no specific message type object for '{MessageType}'!", inputMessage.MessageType.ToString());
            return inputMessage;
        }
        
        try
        {
            return Activator.CreateInstance(classType, inputMessage.Message) as BiDiBInputMessage;
        }
        catch (Exception e) when(e is MissingMethodException or TargetInvocationException)
        {
            logger.LogError(e, "Specific message type object for '{MessageType}' could not be created!", inputMessage.MessageType.ToString());
            return inputMessage;
        }
    }

    private static Type GetInputMessageType(BiDiBMessage messageType)
    {
        return MessageTypeMappings.TryGetValue(messageType, out var mapping) ? mapping : null;
    }

    private static void GetTypeMappings()
    {

        var currentNamespace = typeof(BiDiBInputMessage).Namespace;
        var classes = typeof(BiDiBInputMessage).Assembly.GetTypes().Where(t => t.IsClass && t.Namespace == currentNamespace).ToList();

        foreach (var inputClass in classes)
        {
            if (Attribute.GetCustomAttribute(inputClass, typeof(InputMessageAttribute)) is InputMessageAttribute att && !MessageTypeMappings.ContainsKey(att.MessageType))
            {
                MessageTypeMappings.Add(att.MessageType, inputClass);
            }
        }
    }
}