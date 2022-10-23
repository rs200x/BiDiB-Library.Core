using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Controllers;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Controllers
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class SocketControllerTests : TestClass<SocketController>
    {
        private readonly byte[] data = new byte[50];

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            var loggerFactory = new Mock<ILoggerFactory>();
            Target = new SocketController(loggerFactory.Object);
        }

        //[TestMethod]
        public void Test()
        {
            // Arrange
            IPEndPoint target = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 62875);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(target);

            // schedule the first receive operation:
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnUdpData), socket);

            byte[] message = BiDiBMessageGenerator.GenerateMessage(new byte[] {0}, BiDiBMessage.MSG_SYS_GET_MAGIC);
            byte[] msgToSend = new byte[message.Length + 4];

            Array.Copy(message, 0, msgToSend, 4, message.Length);

           

            socket.Send(msgToSend);
            Thread.Sleep(500);
            socket.Send(msgToSend);
            Thread.Sleep(500);

            // Assert
            socket.Disconnect(false);
        }

        private void OnUdpData(IAsyncResult result)
        {
            // this is what had been passed into BeginReceive as the second parameter:
            Socket socket = result.AsyncState as Socket;
            // points towards whoever had sent the message:
            EndPoint source = new IPEndPoint(0, 0);
            // get the actual message and fill out the source:
            int message = socket.EndReceive(result);
            // do what you'd like with `message` here:
            Console.WriteLine("Got " + message + " bytes from " + source);
            Console.WriteLine($"Socket data received {BitConverter.ToString(data, 0, message)}");
            byte[] messageData = new byte[data.Length - 4];
            Array.Copy(data, 4, messageData, 0, data.Length - 4);

            // schedule the next receive operation once reading is done:
            try
            {
                socket.BeginReceive(data, 0, data.Length, SocketFlags.None,  new AsyncCallback(OnUdpData), socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}