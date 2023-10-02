using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Models.Messages.Output;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Message;

/// <summary>
/// The message service is the main instance processing incoming and outgoing bidib messages
/// Incoming messages get transformed into <see cref="BiDiBInputMessage"/> and provided to all <see cref="IMessageReceiver"/>
/// Outgoing messages get queued and send to the interface
/// </summary>
[Localizable(false)]
[Export(typeof(IBiDiBMessageService))]
[PartCreationPolicy(CreationPolicy.Shared)]
public sealed class BiDiBMessageService : IBiDiBMessageService
{
    private readonly IConnectionService connectionService;
    private readonly IBiDiBMessageExtractor messageExtractor;
    private readonly ILogger<BiDiBMessageService> logger;
    private readonly ILogger serviceLogger;
    private readonly BlockingCollection<BiDiBInputMessage> messageQueue;
    private readonly BlockingCollection<BiDiBOutputMessage> outputMessageQueue;
    private readonly ConcurrentDictionary<int, byte> addressSequenceNumbers;
    private readonly ConcurrentDictionary<int, IMessageReceiver> messageReceivers;
    private static readonly byte[] DefaultInterfaceAddress = { 0 };
    private CancellationTokenSource cancellationTokenSource;
    private Task inputQueueTask;
    private Task outputQueueTask;
    private readonly Stopwatch inputProcessWatch;

    [ImportingConstructor]
    public BiDiBMessageService(IConnectionService connectionService, IBiDiBMessageExtractor messageExtractor, ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<BiDiBMessageService>();
        serviceLogger = loggerFactory.CreateLogger(BiDiBConstants.LoggerContextMessage);

        this.connectionService = connectionService;
        this.connectionService.DataReceived += ProcessMessage;
        this.messageExtractor = messageExtractor;

        messageQueue = new BlockingCollection<BiDiBInputMessage>(new ConcurrentQueue<BiDiBInputMessage>());
        outputMessageQueue = new BlockingCollection<BiDiBOutputMessage>(new ConcurrentQueue<BiDiBOutputMessage>());

        addressSequenceNumbers = new ConcurrentDictionary<int, byte>();
        messageReceivers = new ConcurrentDictionary<int, IMessageReceiver>();

        inputProcessWatch = new Stopwatch();
    }

    public bool IsActive { get; internal set; }

    public void Activate()
    {
        IsActive = true;
        cancellationTokenSource = new CancellationTokenSource();

        // init processing
        inputQueueTask = Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
        outputQueueTask = Task.Factory.StartNew(ProcessOutputQueue, TaskCreationOptions.LongRunning);
    }

    public void Deactivate()
    {
        if (!IsActive) { return; }

        IsActive = false;
        cancellationTokenSource.Cancel();
        Task.WaitAll(inputQueueTask, outputQueueTask);
        ClearQueue();
    }

    public void Cleanup()
    {
        ClearQueue();
        addressSequenceNumbers.Clear();
    }

    #region handle incoming messages

    public void ProcessMessage(byte[] messageBytes)
    {
        try
        {
            var inputMessages = messageExtractor.ExtractMessage(messageBytes, connectionService.MessageSecurity);

            foreach (var inputMessage in inputMessages)
            {
                LogMessageInput(inputMessage);
                messageQueue.Add(inputMessage);
            }
        }
        catch (Exception e) when (e is ArgumentOutOfRangeException or IndexOutOfRangeException)
        {
            logger.LogError(e, "Message bytes could not be processed {DataString}", messageBytes.GetDataString());
        }
    }

    private void ProcessQueue()
    {
        while (IsActive)
        {
            if (cancellationTokenSource.IsCancellationRequested) { continue; }

            BiDiBInputMessage messageItem;

            try
            {
                if (!messageQueue.TryTake(out messageItem, 20, cancellationTokenSource.Token))
                {
                    continue;
                }
            }
            catch (OperationCanceledException operationCanceled)
            {
                logger.LogDebug("Input message queue processing: {Message}", operationCanceled.Message);
                continue;
            }

            ProcessInputMessage(messageItem);
        }

        logger.LogDebug("Input message queue processing finished");
    }

    private void ProcessInputMessage(BiDiBInputMessage messageItem)
    {
        var currentReceivers = messageReceivers.Values.ToList(); // take a copy to avoid modification

        inputProcessWatch.Restart();
        foreach (var receiver in currentReceivers)
        {
            try
            {
                receiver.ProcessMessage(messageItem);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Message could not be processed by {ReceiverName}. {MessageItem}", receiver.GetType().Name, messageItem);
            }
        }

        inputProcessWatch.Stop();

        if (inputProcessWatch.ElapsedMilliseconds > 100)
        {
            logger.LogWarning("Message {MessageType} processed ({ElapsedMilliseconds}ms)", messageItem.MessageType, inputProcessWatch.ElapsedMilliseconds);
        }
    }

    private void ProcessOutputQueue()
    {
        while (IsActive)
        {

            if (cancellationTokenSource.IsCancellationRequested) { continue; }

            BiDiBOutputMessage outputMessageItem;

            try
            {
                outputMessageItem = outputMessageQueue.Take(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException operationCanceled)
            {
                logger.LogDebug("Output message queue processing: {Message}", operationCanceled.Message);
                continue;
            }

            if (outputMessageItem == null || cancellationTokenSource.IsCancellationRequested)
            {
                continue;
            }

            var messageBytes = outputMessageItem is MultiOutputMessage multiMessage
                ? BiDiBMessageGenerator.GenerateMessage(multiMessage.Messages)
                : BiDiBMessageGenerator.GenerateMessage(outputMessageItem);

            var size = GetMessageSize(messageBytes);

            LogMessageOutput(outputMessageItem, messageBytes);
            connectionService.SendData(messageBytes, size);
        }

        logger.LogDebug("Output message queue processing finished");
    }

    private void ClearQueue()
    {
        while (messageQueue.Count > 0)
        {
            messageQueue.TryTake(out _, 10);
        }

        while (outputMessageQueue.Count > 0)
        {
            outputMessageQueue.TryTake(out _, 10);
        }
    }

    #endregion

    #region handle outgoing messages

    public void SendMessage(BiDiBMessage messageType, byte[] address, params byte[] parameters)
    {
        SendMessage(new BiDiBOutputMessage(address ?? DefaultInterfaceAddress, messageType, parameters));
    }

    public void SendMessage(BiDiBOutputMessage outputMessage)
    {
        if (outputMessage == null)
        {
            throw new ArgumentNullException(nameof(outputMessage));
        }
        
        outputMessage.SequenceNumber = GetNextSequenceNumber(outputMessage);
        AddToOutputQueue(outputMessage);
    }

    public void SendMessages(ICollection<BiDiBOutputMessage> outputMessages)
    {
        foreach (var outputMessage in outputMessages)
        {
            outputMessage.SequenceNumber = GetNextSequenceNumber(outputMessage);
        }

        AddToOutputQueue(outputMessages.Count == 1 ? outputMessages.First() : new MultiOutputMessage(outputMessages));
    }

    private void AddToOutputQueue(BiDiBOutputMessage outputMessage)
    {
        if (connectionService.ConnectionState.InterfaceState == InterfaceConnectionState.Disconnected)
        {
            logger.LogError("Interface is not connected! Message will not be transmitted!");
            return;
        }

        if (!outputMessageQueue.TryAdd(outputMessage))
        {
            logger.LogError("Adding message to output queue failed {Message}", outputMessage);
            outputMessageQueue.Add(outputMessage);
        }
    }

    private static int GetMessageSize(IList<byte> messageBytes)
    {
        var lastDataIndex = 0;
        for (var i = 0; i < messageBytes.Count; i++)
        {
            var data = messageBytes[i];
            if (data != 0)
            {
                lastDataIndex = i;
            }
        }

        return lastDataIndex + 1;
    }

    public void ResetMessageSequenceNumber(byte[] address)
    {
        if (addressSequenceNumbers.ContainsKey(address.GetArrayValue()))
        {
            addressSequenceNumbers[address.GetArrayValue()] = 0;
        }
    }

    private byte GetNextSequenceNumber(BiDiBOutputMessage outputMessage)
    {
        if (outputMessage.MessageType >= BiDiBMessage.MSG_LOCAL_LOGON)
        {
            return 0;
        }
            
        var addressNumber = outputMessage.Address.GetArrayValue();
        var newSequenceNumber = addressSequenceNumbers.AddOrUpdate(addressNumber, 0, (_, oldValue) => (byte)(oldValue == 0xff ? 1 : oldValue + 1));

        return newSequenceNumber;
    }

    #endregion

    #region logging

    private void LogMessageInput(BiDiBInputMessage inputMessage)
    {
        logger.LogDebug("{Message}", inputMessage);
        serviceLogger.LogDebug("{Message} {DataString}", inputMessage, inputMessage.Message.GetDataString());
    }

    private void LogMessageOutput(BiDiBOutputMessage outputMessage, byte[] messageBytes)
    {
        logger.LogDebug("{Message}", outputMessage);

        if (outputMessage is MultiOutputMessage multiMessage)
        {
            serviceLogger.LogDebug("{MessagePrefix} {Type,-25} {DataString}", BiDiBConstants.OutMessagePrefix, "\"MULTI_MESSAGE\"", messageBytes.GetDataString());
            foreach (var message in multiMessage.Messages)
            {
                serviceLogger.LogDebug("{Message} {DataString}", message, message.GetMessageBytes().GetDataString());
            }
        }
        else
        {
            serviceLogger.LogDebug("{Message} {DataString}", outputMessage, outputMessage.GetMessageBytes().GetDataString());
        }
    }

    public void Register(IMessageReceiver messageReceiver)
    {
        if (!messageReceivers.ContainsKey(messageReceiver.GetHashCode()))
        {
            messageReceivers.AddOrUpdate(messageReceiver.GetHashCode(), messageReceiver, (_, receiver) => receiver);
        }
    }

    public void Unregister(IMessageReceiver messageReceiver)
    {
        messageReceivers.TryRemove(messageReceiver.GetHashCode(), out _);
    }

    #endregion

    public void Dispose()
    {
        Cleanup();
        connectionService.DataReceived -= ProcessMessage;
        messageReceivers?.Clear();
        messageQueue?.Dispose();
        outputMessageQueue?.Dispose();
        cancellationTokenSource?.Dispose();
        inputQueueTask?.Dispose();
        outputQueueTask?.Dispose();
    }
}