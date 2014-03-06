using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CgminerMonitorClient.Workers.Cgminer
{
    public class CommandSender
    {
        public static string SendMessage(string message, int port)
        {
            var bytesReceived = new byte[256];
            var socketPermission = new SocketPermission(NetworkAccess.Connect, TransportType.Tcp, "", -1);
            socketPermission.Demand();
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var remoteEndIp = new IPEndPoint(ipAddress, port);

            using (var socket = new Socket(remoteEndIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.NoDelay = false;
                socket.SendTimeout = (int)TimeSpan.FromSeconds(20).TotalMilliseconds;
                socket.ReceiveTimeout = (int)TimeSpan.FromSeconds(20).TotalMilliseconds;
                socket.Connect(remoteEndIp);
                var bytesSent = Encoding.UTF8.GetBytes(message);
                socket.Send(bytesSent);
                int bytesRead;
                var resultSb = new StringBuilder();
                do
                {
                    bytesRead = socket.Receive(bytesReceived, bytesReceived.Length, 0);
                    resultSb.Append(Encoding.UTF8.GetString(bytesReceived, 0, bytesRead));
                }
                while (bytesRead > 0);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return resultSb.ToString();
            }
        }
    }
}