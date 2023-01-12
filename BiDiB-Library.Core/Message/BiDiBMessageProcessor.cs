using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.BiDiB;
using org.bidib.netbidibc.core.Models.BiDiB.Base;
using org.bidib.netbidibc.core.Models.BiDiB.Extensions;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Models.Messages.Output;
using org.bidib.netbidibc.core.Services.Interfaces;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Message
{
    [Export(typeof(IBiDiBMessageProcessor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BiDiBMessageProcessor : IBiDiBMessageProcessor
    {
        private readonly ILogger<BiDiBMessageProcessor> logger;
        private readonly IBiDiBNodesFactory nodesFactory;
        private readonly IBiDiBMessageService messageService;
        private const int DefaultTimeout = 500;

        [ImportingConstructor]
        public BiDiBMessageProcessor(IBiDiBNodesFactory nodesFactory, IBiDiBMessageService messageService,ILoggerFactory loggerFactory)
        {
            this.nodesFactory = nodesFactory;
            this.messageService = messageService;
            logger = loggerFactory.CreateLogger<BiDiBMessageProcessor>();
        }

        public void GetChildNodes(byte[] parentAddress)
        {
            BiDiBNode parentNode = nodesFactory.GetNode(parentAddress);
            if (parentNode == null) { return; }

            NodeTabCountMessage nodeTabCountMessage = SendMessage<NodeTabCountMessage>(parentNode, BiDiBMessage.MSG_NODETAB_GETALL);
            if (nodeTabCountMessage == null) { return; }

            int retryCount = 0;
            while (nodeTabCountMessage.NodeCount == 0 && retryCount < 5)
            {
                logger.LogDebug($"node {parentNode.FullAddress} is still initializing. Wait and try again");
                Thread.Sleep(DefaultTimeout);
                nodeTabCountMessage = SendMessage<NodeTabCountMessage>(parentNode, BiDiBMessage.MSG_NODETAB_GETALL);
                retryCount++;
            }

            if (nodeTabCountMessage.NodeCount == 0)
            {
                logger.LogWarning($"node {parentNode.FullAddress} is still initializing after {retryCount + 1}");
                return;
            }

            for (int i = 0; i < nodeTabCountMessage.NodeCount; i++)
            {
                NodeTabMessage nodeTabMessage = SendMessage<NodeTabMessage>(parentNode, BiDiBMessage.MSG_NODETAB_GETNEXT);
                if (nodeTabMessage == null) { continue; }

                BiDiBNode node = nodesFactory.GetNode(nodeTabMessage.NodeAddress)
                    ?? nodesFactory.CreateNode(nodeTabMessage.NodeAddress, nodeTabMessage.UniqueId);

                logger.LogDebug($"Node detected UID: {node.HexUniqueId}, ADDR: {node.GetFullAddressString()}, VPID: {node.VendorProductId}");
            }
        }

        public TResponseMessage SendMessage<TResponseMessage>(BiDiBNode node, BiDiBMessage messageType, params byte[] parameters)
            where TResponseMessage : BiDiBInputMessage
        {
            return SendMessage<TResponseMessage>(node, messageType, DefaultTimeout, parameters);
        }

        public TResponseMessage SendMessage<TResponseMessage>(BiDiBNode node, BiDiBMessage messageType, int timeout, params byte[] parameters)
            where TResponseMessage : BiDiBInputMessage
        {
            var messages = new List<BiDiBOutputMessage> { new(node.Address, messageType, parameters) };
            return SendMessageMultiResponseMessage<TResponseMessage>(node.Address, messages, 1, timeout).FirstOrDefault();
        }

        public TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage)
            where TResponseMessage : BiDiBInputMessage
        {
            return SendMessage<TResponseMessage>(outputMessage, DefaultTimeout);
        }

        public TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage, int timeout)
            where TResponseMessage : BiDiBInputMessage
        {
            return outputMessage == null ?
                default
                : SendMessageMultiResponseMessage<TResponseMessage>(outputMessage.Address, new List<BiDiBOutputMessage> { outputMessage }, 1, timeout)
                    .FirstOrDefault();
        }

        public IEnumerable<TResponseMessage> SendMessages<TResponseMessage>(ICollection<BiDiBOutputMessage> outputMessages, int timeout = 300)
            where TResponseMessage : BiDiBInputMessage
        {
            if (outputMessages == null || !outputMessages.Any())
            {
                return Enumerable.Empty<TResponseMessage>();
            }

            return SendMessageMultiResponseMessage<TResponseMessage>(outputMessages.First().Address, outputMessages, outputMessages.Count, timeout);
        }

        public void SendMessage(BiDiBOutputMessage outputMessage)
        {
            messageService.SendMessage(outputMessage);
        }

        public void SendMessage(BiDiBNode node, BiDiBMessage messageType, params byte[] parameters)
        {
            messageService.SendMessage(messageType, node.Address, parameters);
        }

        public IEnumerable<Port> GetPorts(BiDiBNode node, PortType portType, int expectedItems)
        {
            var outputMessages = new List<BiDiBOutputMessage> { new ConfigXGetAllMessage(node.Address, portType, 0, portType, (byte)expectedItems) };
            IEnumerable<LcConfigXMessage> configXMessages = SendMessageMultiResponseMessage<LcConfigXMessage>(node.Address, outputMessages, expectedItems);
            return configXMessages.Select(configXMessage => GetNewPort(portType, configXMessage.PortNumber)).Where(port => port != null).ToList();
        }

        private IEnumerable<TResponseMessage> SendMessageMultiResponseMessage<TResponseMessage>(byte[] address,
            ICollection<BiDiBOutputMessage> outputMessages,
            int expectedMessages,
            int timeout = DefaultTimeout)
            where TResponseMessage : BiDiBInputMessage
        {
            bool timedout = false;
            int receivedCount = 0;
            ManualResetEventSlim manualReset = new();
            List<TResponseMessage> responseMessages = new();

            RuntimeMessageReceiver<TResponseMessage> messageReceiver = new(x =>
            {
                if (x.Address.GetArrayValue() != address.GetArrayValue())
                {
                    logger.LogDebug($"Skip processing message of other node {NodeExtensions.GetFullAddressString(x.Address)} -> {NodeExtensions.GetFullAddressString(address)}");
                    return;
                }

                responseMessages.Add(x);
                manualReset.Set();
            });

            messageService.Register(messageReceiver);

            messageService.SendMessages(outputMessages);
            while (receivedCount < expectedMessages && !timedout)
            {
                manualReset.Reset();
                if (!manualReset.Wait(timeout))
                {
                    timedout = true;
                    logger.LogWarning($"Response timeout for {string.Join(";", outputMessages)} ({responseMessages.Count}/{expectedMessages})");
                }

                receivedCount = responseMessages.Count;
            }

            messageService.Unregister(messageReceiver);
            return responseMessages;
        }

        private static Port GetNewPort(PortType portType, byte portNumber)
        {
            switch (portType)
            {
                case PortType.Switch: { return new OutputSwitch { Number = portNumber }; }
                case PortType.Light: { return new OutputLight { Number = portNumber }; }
                case PortType.Servo: { return new OutputServo { Number = portNumber }; }
                case PortType.Sound: { return new OutputSound { Number = portNumber }; }
                case PortType.Motor: { return new OutputMotor { Number = portNumber }; }
                case PortType.AnalogOut: { return new OutputAnalog { Number = portNumber }; }
                case PortType.Backlight: { return new OutputBacklight { Number = portNumber }; }
                case PortType.Switchpair:
                    break;
                case PortType.Input: { return new InputKey { Number = portNumber }; }
                case PortType.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(portType), portType, null);
            }

            return null;
        }
    }
}