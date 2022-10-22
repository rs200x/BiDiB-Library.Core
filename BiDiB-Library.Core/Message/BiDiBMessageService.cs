using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Models.Messages.Output;
using org.bidib.netbidibc.core.Properties;
using org.bidib.netbidibc.core.Services.Interfaces;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Message
{
    /// <summary>
    /// The message service is the main instance processing incoming and outgoing bidib messages
    /// Incoming messages get transformed into <see cref="BiDiBInputMessage"/> and provided to all <see cref="IMessageReceiver"/>
    /// Outgoing messages get queued and send to the interface
    /// </summary>
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

        public BiDiBMessageService(IConnectionService connectionService, IBiDiBMessageExtractor messageExtractor, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<BiDiBMessageService>();
            this.serviceLogger = loggerFactory.CreateLogger("MS");

            this.connectionService = connectionService;
            this.connectionService.DataReceived += ProcessMessage;
            this.messageExtractor = messageExtractor;

            messageQueue = new BlockingCollection<BiDiBInputMessage>();
            outputMessageQueue = new BlockingCollection<BiDiBOutputMessage>();

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
            catch (Exception e) when (e is ArgumentOutOfRangeException || e is IndexOutOfRangeException)
            {
                logger.LogError(e, $"Message bytes could not be processed {messageBytes.GetDataString()}");
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
                    logger.LogDebug(Resources.InputQueueProcessingCanceled, operationCanceled);
                    continue;
                }

                ProcessInputMessage(messageItem);
            }

            logger.LogDebug(Resources.InputQueueProcessingFinished);
        }

        private void ProcessInputMessage(BiDiBInputMessage messageItem)
        {
            List<IMessageReceiver> currentReceivers = messageReceivers.Values.ToList(); // take a copy to avoid modification

            inputProcessWatch.Restart();
            foreach (IMessageReceiver receiver in currentReceivers)
            {
                try
                {
                    receiver.ProcessMessage(messageItem);
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Message could not be processed by {receiver.GetType().Name}. {messageItem}");
                }
            }

            inputProcessWatch.Stop();

            if (inputProcessWatch.ElapsedMilliseconds > 100)
            {
                logger.LogWarning($"Message {messageItem.MessageType} processed ({inputProcessWatch.ElapsedMilliseconds}ms)");
            }
        }

        private void ProcessOutputQueue()
        {
            while (IsActive)
            {
                BiDiBOutputMessage outputMessageItem;

                try
                {
                    outputMessageItem = outputMessageQueue.Take(cancellationTokenSource.Token);
                }
                catch (OperationCanceledException operationCanceled)
                {
                    logger.LogDebug(Resources.OutputQueueProcessingCanceled, operationCanceled);
                    continue;
                }

                if (outputMessageItem == null || cancellationTokenSource.IsCancellationRequested)
                {
                    continue;
                }

                var messageBytes = outputMessageItem is MultiOutputMessage multiMessage
                    ? BiDiBMessageGenerator.GenerateMessage(multiMessage.Messages)
                    : BiDiBMessageGenerator.GenerateMessage(outputMessageItem);

                int size = GetMessageSize(messageBytes);

                LogMessageOutput(outputMessageItem, messageBytes);
                connectionService.SendData(messageBytes, size);
            }

            logger.LogDebug(Resources.OutputQueueProcessingFinished);
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
            outputMessage.SequenceNumber = GetNextSequenceNumber(outputMessage.Address);
            AddToOutputQueue(outputMessage);
        }

        public void SendMessages(ICollection<BiDiBOutputMessage> outputMessages)
        {
            foreach (var outputMessage in outputMessages)
            {
                outputMessage.SequenceNumber = GetNextSequenceNumber(outputMessage.Address);
            }

            AddToOutputQueue(outputMessages.Count == 1 ? outputMessages.First() : new MultiOutputMessage(outputMessages));
        }

        private void AddToOutputQueue(BiDiBOutputMessage outputMessage)
        {
            if (connectionService.ConnectionState.InterfaceState == InterfaceConnectionState.Disconnected)
            {
                logger.LogError(Resources.InterfaceNotConnectedNoTransmission);
                return;
            }

            outputMessageQueue.Add(outputMessage);
        }

        private static int GetMessageSize(IList<byte> messageBytes)
        {
            int lastDataIndex = 0;
            for (int i = 0; i < messageBytes.Count; i++)
            {
                byte data = messageBytes[i];
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

        private byte GetNextSequenceNumber(byte[] address)
        {
            int addressNumber = address.GetArrayValue();
            byte newSequenceNumber = addressSequenceNumbers.AddOrUpdate(addressNumber, 0, (_, oldValue) => (byte)(oldValue == 0xff ? 1 : oldValue + 1));

            return newSequenceNumber;
        }

        #endregion

        #region logging

        private void LogMessageInput(BiDiBInputMessage inputMessage)
        {
            logger.LogDebug(inputMessage.ToString());
            serviceLogger.LogDebug($"{inputMessage}  {inputMessage.Message.GetDataString()}");
        }

        private void LogMessageOutput(BiDiBOutputMessage outputMessage, byte[] messageBytes)
        {
            logger.LogDebug(outputMessage.ToString());

            if (outputMessage is MultiOutputMessage multiMessage)
            {
                serviceLogger.LogDebug($"{BiDiBConstants.OutMessagePrefix} {"MULTI_MESSAGE",-25} {messageBytes.GetDataString()}");
                foreach (var message in multiMessage.Messages)
                {
                    serviceLogger.LogDebug($"{message}  {message.GetMessageBytes().GetDataString()}");
                }
            }
            else
            {
                serviceLogger.LogDebug($"{outputMessage}  {outputMessage.GetMessageBytes().GetDataString()}");
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
}