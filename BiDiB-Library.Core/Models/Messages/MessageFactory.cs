using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages
{
    public static class MessageFactory
    {
        private static ILogger logger;
        private static readonly IDictionary<BiDiBMessage, Type> messageTypeMappings = new Dictionary<BiDiBMessage, Type>();

        static MessageFactory()
        {
            logger = LoggerFactory.Create(c => c.SetMinimumLevel(LogLevel.Debug)).CreateLogger(typeof(MessageFactory));
            GetTypeMappings();
        }

        /// <summary>
        /// Creates specific input message object of provided message
        /// </summary>
        /// <param name="messageBytes">the message bytes</param>
        /// <returns>the message object, null on any error</returns>
        public static BiDiBInputMessage CreateInputMessage(byte[] messageBytes)
        {
            BiDiBInputMessage inputMessage;

            try
            {
                inputMessage = new BiDiBInputMessage(messageBytes);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Message '{messageBytes.GetDataString()}' could not be converted into generic input message");
                return null;
            }

            try
            {
                inputMessage = GetSpecific(inputMessage);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Message '{messageBytes.GetDataString()}' could not be converted into specific input message");
            }

            return inputMessage;
        }

        private static BiDiBInputMessage GetSpecific(BiDiBInputMessage inputMessage)
        {

            var classType = GetInputMessageType(inputMessage.MessageType);
            if(classType != null)
            {
                return Activator.CreateInstance(classType, new object[] { inputMessage.Message }) as BiDiBInputMessage;
            }


            byte[] messageBytes = inputMessage.Message;
            switch (inputMessage.MessageType)
            {
                case BiDiBMessage.MSG_CS_STATE: { return new CommandStationStateMessage(messageBytes); }
                case BiDiBMessage.MSG_CS_PROG_STATE: { return new CommandStationProgStateMessage(messageBytes); }
                case BiDiBMessage.MSG_CS_POM_ACK: { return new CommandStationPoMAckMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_ERROR: { return new SysErrorMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_MAGIC: { return new SysMagicMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_GET_MAGIC: { return new SysGetMagicMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_UNIQUE_ID: { return new SysUniqueIdMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_SW_VERSION: { return new SysSoftwareMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_GET_SW_VERSION: { return new SysGetSoftwareMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_P_VERSION: { return new SysProtocolMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_GET_P_VERSION: { return new SysGetProtocolMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_IDENTIFY_STATE: { return new SysIdentifyStateMessage(messageBytes); }
                case BiDiBMessage.MSG_SYS_PING: { return new SysPingMessage(messageBytes); }
                case BiDiBMessage.MSG_BOOST_DIAGNOSTIC: { return new BoostDiagnosticMessage(messageBytes); }
                case BiDiBMessage.MSG_BOOST_STAT: { return new BoostStatMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_CV: { return new FeedbackCvMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_OCC: { return new FeedbackOccupiedMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_CONFIDENCE: { return new FeedbackConfidenceMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_FREE: { return new FeedbackMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_DYN_STATE: { return new FeedbackDynStateMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_SPEED: { return new FeedbackSpeedMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_ADDRESS: { return new FeedbackAddressMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_MULTIPLE: { return new FeedbackMultipleMessage(messageBytes); }
                case BiDiBMessage.MSG_BM_POSITION: { return new FeedbackPositionMessage(messageBytes); }
                case BiDiBMessage.MSG_NODETAB: { return new NodeTabMessage(messageBytes); }
                case BiDiBMessage.MSG_NODETAB_COUNT: { return new NodeTabCountMessage(messageBytes); }
                case BiDiBMessage.MSG_NODE_NEW: { return new NodeNewMessage(messageBytes); }
                case BiDiBMessage.MSG_NODE_LOST: { return new NodeLostMessage(messageBytes); }
                case BiDiBMessage.MSG_STRING: { return new StringMessage(messageBytes); }
                case BiDiBMessage.MSG_STRING_GET: { return new StringGetMessage(messageBytes); }
                case BiDiBMessage.MSG_FEATURE_COUNT: { return new FeatureCountMessage(messageBytes); }
                case BiDiBMessage.MSG_FEATURE: { return new FeatureMessage(messageBytes); }
                case BiDiBMessage.MSG_FEATURE_NA: { return new FeatureNaMessage(messageBytes); }
                case BiDiBMessage.MSG_FEATURE_GETALL: { return new FeatureGetAllMessage(messageBytes); }
                case BiDiBMessage.MSG_STALL: { return new NodeStallMessage(messageBytes); }
                case BiDiBMessage.MSG_FW_UPDATE_STAT: { return new FirmwareUpdateStatusMessage(messageBytes); }
                case BiDiBMessage.MSG_VENDOR: { return new VendorMessage(messageBytes); }
                case BiDiBMessage.MSG_VENDOR_ACK: { return new VendorAckMessage(messageBytes); }
                case BiDiBMessage.MSG_ACCESSORY_PARA: { return new AccessoryParaMessage(messageBytes); }
                case BiDiBMessage.MSG_ACCESSORY_STATE: { return new AccessoryStateMessage(messageBytes); }
                case BiDiBMessage.MSG_ACCESSORY_NOTIFY: { return new AccessoryStateMessage(messageBytes); }
                case BiDiBMessage.MSG_LC_CONFIGX: { return new LcConfigXMessage(messageBytes); }
                case BiDiBMessage.MSG_LC_STAT: { return new LcStatMessage(messageBytes); }
                case BiDiBMessage.MSG_LOCAL_LINK: { return new LocalLinkMessage(messageBytes); }
                case BiDiBMessage.MSG_LOCAL_PROTOCOL_SIGNATURE: { return new ProtocolSignatureMessage(messageBytes); }
                case BiDiBMessage.MSG_LOCAL_LOGON: { return new LocalLogonMessage(messageBytes); }
                case BiDiBMessage.MSG_LOCAL_LOGON_ACK: { return new LocalLogonAckMessage(messageBytes); }
                case BiDiBMessage.MSG_LOCAL_LOGOFF: { return new LocalLogoffMessage(messageBytes); }
                case BiDiBMessage.MSG_LOCAL_PING: { return new LocalPingMessage(messageBytes); }
                case BiDiBMessage.MSG_GUEST_RESP_NOTIFY: { return new GuestResponseNotifyMessage(messageBytes); }
                case BiDiBMessage.MSG_GUEST_RESP_SENT: { return new GuestResponseSentMessage(messageBytes); }
                case BiDiBMessage.MSG_GUEST_RESP_SUBSCRIPTION: { return new GuestResponseSubscriptionMessage(messageBytes); }

                default:
                {
                    return inputMessage;
                }
            }
        }

        private static Type GetInputMessageType(BiDiBMessage messageType)
        {
            if (messageTypeMappings.ContainsKey(messageType))
            {
                return messageTypeMappings[messageType];
            }

            return null;

        }

        private static void GetTypeMappings()
        {

            var currentNamespace = typeof(MessageFactory).Namespace;

            var classes = typeof(MessageFactory).Assembly.GetTypes().Where(t => t.IsClass && t.Namespace == currentNamespace + ".Input");

            foreach (var inputClass in classes)
            {
                var att = Attribute.GetCustomAttribute(inputClass, typeof(InputMessageAttribute)) as InputMessageAttribute;

                if (att != null && !messageTypeMappings.ContainsKey(att.MessageType))
                {
                    messageTypeMappings.Add(att.MessageType, inputClass);
                }
            }
        }
    }
}