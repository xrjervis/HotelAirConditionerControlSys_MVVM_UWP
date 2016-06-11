using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Slave.Services
{
    public class TcpClient
    {
        //Create the StreamSocket and establish a connection to the echo server.
        Windows.Networking.Sockets.StreamSocket socket = new Windows.Networking.Sockets.StreamSocket();

        //单例模式
        public static TcpClient TcpConnection;

        public static TcpClient GetInstance()
        {
            if(TcpConnection == null)
            {
                TcpConnection = new TcpClient();
            }
            return TcpConnection;
        }

        public async Task Start()
        {
            try
            {
                //The server hostname that we will be establishing a connection to. We will be running the server and client locally,
                //so we will use localhost as the hostname.
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName("127.0.0.1");

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                //For the echo server/client application we will use a random port 8080.
                string serverPort = "8080";
                await socket.ConnectAsync(serverHost, serverPort);

            }
            catch (Exception e)
            {
                //Handle exception here. 
            }

        }

        public async Task SendToTcpServer(string message)
        {
            try
            {
                //Write data to the echo server.
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(streamOut, Encoding.GetEncoding("utf-8"));
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
            catch (Exception e)
            {
                //Handle exception here. 
            }
        }
        
        //返回值是String
        public async Task<string> WaitTcpServer()
        {
            try
            {
                //Read data from the echo server.
                Stream streamIn = socket.InputStream.AsStreamForRead();
                StreamReader reader = new StreamReader(streamIn);
                string response = await reader.ReadLineAsync();

                Debug.WriteLine(response);
                return response;
            }
            catch (Exception e)
            {
                //Handle exception here. 
                return null;
            }
        }
    }
}
