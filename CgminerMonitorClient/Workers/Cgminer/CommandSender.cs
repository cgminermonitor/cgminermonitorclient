using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers.Cgminer
{
    public class CommandSender
    {
        public static string SendMessage(string message, string ip, int port)
        {
            var bytesReceived = new byte[256];
            var socketPermission = new SocketPermission(NetworkAccess.Connect, TransportType.Tcp, "", -1);
            socketPermission.Demand();
            var ipAddress = IPAddress.Parse(ip);
            var remoteEndIp = new IPEndPoint(ipAddress, port);
            using (var socket = new Socket(remoteEndIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.NoDelay = false;
                socket.SendTimeout = (int)TimeSpan.FromSeconds(20).TotalMilliseconds;
                socket.ReceiveTimeout = (int)TimeSpan.FromSeconds(20).TotalMilliseconds;

                //Log.Instance.DebugFormat("Connecting to {0}", remoteEndIp);
                socket.Connect(remoteEndIp);
                var bytesSent = Encoding.UTF8.GetBytes(message);

                //Log.Instance.Debug("Sending message...");
                socket.Send(bytesSent);
                int bytesRead;
                var resultSb = new StringBuilder();
                do
                {
                    bytesRead = socket.Receive(bytesReceived, bytesReceived.Length, 0);
                    resultSb.Append(Encoding.UTF8.GetString(bytesReceived, 0, bytesRead));
                }
                while (bytesRead > 0);
                //Log.Instance.Debug("Received response.");

                try
                {
                    //Log.Instance.Debug("Calling socket.Shutdown.");
                    socket.Shutdown(SocketShutdown.Both);
                    //Log.Instance.Debug("Calling socket.Close.");
                    socket.Close();
                }
                catch (SocketException)
                {
                    if (Consts.Distro != Consts.MacOsxDistroName)
                        throw;
                    Log.Instance.Debug("Closing socket failed. This is bug in Mono on MAC OSX. Continuing...");
                }

                return resultSb.ToString();
            }
        }
    }
}