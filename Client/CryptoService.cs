using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KryptografiaClient
{
    public class CryptoService
    {
        private string _Key;
        public CryptoService()
        {
            _Key = File.ReadAllText("klucz.txt");
            
            

        }
        public string EncryptMessage(string message)
        {
            byte[] encryptedMessage = new byte[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                encryptedMessage[i] = (byte)(message[i] ^ _Key[i]);

            }
            _Key.Remove(0, message.Length);
            return Encoding.ASCII.GetString(encryptedMessage);
        }
        public string DecryptMessage(string message)
        {
            byte[] decryptedMessage = new byte[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                decryptedMessage[i] = (byte)(message[i] ^ _Key[i]);

            }
            _Key.Remove(0, message.Length);
            return Encoding.ASCII.GetString(decryptedMessage);
        }
        public byte[] EncryptMessage(byte[] message)
        {
            byte[] encryptedMessage = new byte[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                encryptedMessage[i] = (byte)(message[i] ^ _Key[i]);

            }
            _Key.Remove(0, message.Length);
            return encryptedMessage;
        }
        public byte[] DecryptMessage(byte[] message)
        {
            byte[] decryptedMessage = new byte[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                decryptedMessage[i] = (byte)(message[i] ^ _Key[i]);

            }
            _Key.Remove(0, message.Length);
            return decryptedMessage;
        }

    }
}
