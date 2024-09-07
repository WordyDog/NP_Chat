using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace finalwork
{
    class Client
    {
        private static string username;
        private static TcpClient client;

        static void Main(string[] args)
        {
            client = new TcpClient("127.0.0.1", 5000);

            Console.Write("Enter your user name: ");
            username = Console.ReadLine();

            NetworkStream stream = client.GetStream();
            byte[] nameBytes = Encoding.UTF8.GetBytes(username);
            stream.Write(nameBytes, 0, nameBytes.Length);

            Thread thread = new Thread(ReceiveMessages);
            thread.Start();

            while (true)
            {
                Console.Write("Enter the recipient's name: ");
                string recipient = Console.ReadLine();
                Console.Write("Enter message: ");
                string message = Console.ReadLine();

                string fullMessage = $"{recipient}:{message}";
                byte[] messageBytes = Encoding.UTF8.GetBytes(fullMessage);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }

        private static void ReceiveMessages()
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
        }
    }
}
