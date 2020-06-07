using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KryptografiaServer
{
    public class Server
    {

        private readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly Dictionary<int, Socket> clientSockets = new Dictionary<int, Socket>();
        private const int BUFFER_SIZE = 512000;
        private const int PORT = 999;
        int counter = 1000;
        private readonly byte[] buffer = new byte[BUFFER_SIZE];
        public void Initialize()
        {
            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(2);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socket;
            try
            {
                socket = serverSocket.EndAccept(ar);
            }catch(Exception e)
            {
                return;
            }
            var id = ++counter;
            if (clientSockets.Count >= 2)
            {
                return;
            }
            clientSockets.Add(id,socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            
            socket.Send(Encoding.ASCII.GetBytes($"L{id}"));
            Console.WriteLine("Client connected, waiting for request...");
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket current = (Socket)ar.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(ar);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                //clientSockets.Remove(clientSockets.)
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Received Text: " + text);
            RedirectMessage(recBuf);
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }

       
        

        private void RedirectMessage(byte[] data)
        {
            // Przekieruj dane czyli do odpowiedniego clienta
            // Informacja do które clienta masz przekierować jest w nagłówku danych np. data= "F1002jdlksajdlksajdlksajdlsajdakl"
            // F - typ danych 
            // 1002 - id wyslającego, czyli wiadomosc trzeba przekierować do np 1001
            // cała reszta - treść
            // Odpowiedniego clienta wezmiesz z _TcpClients np. _TcpClients[1001] 
            string text = Encoding.ASCII.GetString(data);
            if (data[0] == 'F')
            {
                SaveEncryptedFile(data);
            }
            var id = text.Substring(1, 4);
            var reciverId = 0;
            if (id == "1001")
                reciverId = 1002;
            else
                reciverId = 1001;
            var socket = clientSockets[reciverId];

            socket.Send(data,0,data.Length,SocketFlags.None);
        }

        private void SaveEncryptedFile(byte[] data)
        {
            var dataText = Encoding.ASCII.GetString(data);
            var split = dataText.Split("--!");
            var header = split[0];
            var body = data.Skip(header.Length).ToArray();
            var filename = header.Substring(5, header.Length - 5);
            using (var stream = new FileStream("PLIKI ODEBRANE\\" + filename, FileMode.Append, FileAccess.Write))
            {
                
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
