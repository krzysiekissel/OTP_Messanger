using System;

namespace KryptografiaServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Initialize();
            Console.ReadLine();
        }
    }
}
