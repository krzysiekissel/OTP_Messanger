using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KryptografiaClient
{
    public class Client
    {
        private byte[] _Buff = new byte[512000];
        private readonly Socket _ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private CryptoService CryptoProvider= new CryptoService();
        private int _ClientId = 0;
        public delegate void UpdateUi(string data);
        public UpdateUi UpdateDelegate;

        public Client(UpdateUi updateUiMethod)
        {
            UpdateDelegate = updateUiMethod;
        }
        public bool Initialize()
        {
            try
            {
                _ClientSocket.Connect(IPAddress.Loopback, 999);
                Task.Run(Start);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }

        private void Start()
        {
            while (true)
            {
                var buffer = new byte[512000];
                int recived = _ClientSocket.Receive(buffer, SocketFlags.None);
                if (recived == 0)
                    continue;
                var byteData = new byte[recived];
                Array.Copy(buffer, byteData, recived);
                ReciveMessage(byteData);
            }
        }

        public void  SendTextMessage(string body)
        {
            //Zakoduj wiadomosc
            //Wyslij wiadomość na serwer
            var encryptedMessageBody = CryptoProvider.EncryptMessage(body);
            var header = $"M{_ClientId}";
            var message = header + encryptedMessageBody;
            var buffer = Encoding.ASCII.GetBytes(message);
            _ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            
            
        }
        public void SendFileMessage(string path)
        {
            using(var stream = File.OpenRead(path))
            {
                var fileName = path.Split("\\").Last();
                var buffer = new byte[stream.Length];
                stream.Read(buffer);
                var encryptedFileBody = CryptoProvider.EncryptMessage(buffer);
                var header = $"F{_ClientId}{fileName}--!";
                var hederBytes = Encoding.ASCII.GetBytes(header);
                var messageByteList = new List<byte>();
                messageByteList.AddRange(hederBytes);
                messageByteList.AddRange(encryptedFileBody);
                _ClientSocket.Send(messageByteList.ToArray(), 0, messageByteList.Count, SocketFlags.None);
            }
            //zakoduj plik
            //wyslij plik
        }
        public void ReciveMessage(byte[] data)
        {
            if (data.Length == 0)
                return;
            if (data[0] == 'M')
                ReadTextMessage(Encoding.ASCII.GetString(data));
            if (data[0] == 'F')
                ReadFileMessage(data);
            if (data[0] == 'L')
                ReadLogInfo(Encoding.ASCII.GetString(data));
        }

        private void ReadLogInfo(string data)
        {
            _ClientId = Convert.ToInt32(data.Substring(1, 4));
        }

        private void ReadFileMessage(byte[] data)
        {
            var dataText = Encoding.ASCII.GetString(data);
            var split = dataText.Split("--!");
            var header = split[0];
            var body = data.Skip(header.Length+3).ToArray();
            var filename = header.Substring(5, header.Length - 5);
            using(var stream = new FileStream("PLIKI ODEBRANE\\"+filename,FileMode.Append, FileAccess.Write))
            {
                var decryptedBody = CryptoProvider.DecryptMessage(body);
                
                stream.Write(decryptedBody, 0, decryptedBody.Length);
            }
            UpdateDelegate($"Odebrano plik {filename}");
            // F1002plik.txt
            //rozkoduj plik
            //zapisz plik
            //wrzuc wiadomosc na ekran
        }

        private void ReadTextMessage(string data)
        {
            Console.WriteLine(data);
            var decryptedMessageBody = CryptoProvider.DecryptMessage(data.Remove(0, 5));
            UpdateDelegate(decryptedMessageBody);
            //rozkoduj wiadomosc 
            //wrzuc wiadomosc na ekran
            
        }
    }
}
