using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace finalwork
{
    class Server
    {
        private static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        private static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            lock (clients)
            {
                clients[clientName] = client;
            }

            Console.WriteLine($"{clientName} connected to server");

            while (true)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] splitMessage = message.Split(':');
                    string recipientName = splitMessage[0];
                    string actualMessage = splitMessage[1];

                    lock (clients)
                    {
                        if (clients.ContainsKey(recipientName))
                        {
                            TcpClient recipientClient = clients[recipientName];
                            NetworkStream recipientStream = recipientClient.GetStream();
                            byte[] messageBytes = Encoding.UTF8.GetBytes($"{clientName}: {actualMessage}");
                            recipientStream.Write(messageBytes, 0, messageBytes.Length);
                        }
                        else
                        {
                            Console.WriteLine($"{recipientName} offline.");
                        }
                    }
                }
                catch
                {
                    break;
                }
            }

            lock (clients)
            {
                clients.Remove(clientName);
            }

            Console.WriteLine($"{clientName} disconnected from server");
            client.Close();
        }
    }
}
