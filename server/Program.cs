using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    // in my case i used a console application
    class Program
    {
        private static Server server;
        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 3122;
            server = new Server(ip, port);
            server.Cliente_Conneted_Event += server_Cliente_Conneted_Event;
            server.Start();

            // connect client 1
            Client client = new Client(ip, port);
            client.Connect();
            client.Cliente_Message_Event+=client_Cliente_Message_Event;

            sendMessageToClient();
            Console.ReadKey();

        }

        //receive message from server
        private static void client_Cliente_Message_Event(string msg, ClientInfo c)
        {
            try {
                Console.WriteLine("Msg from Server:"+msg);
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }

        // client connected to server
        private static void server_Cliente_Conneted_Event(ClientInfo c)
        {
            Console.WriteLine("Client Connected...");
        }

        // send message to all clients from server
        private static void sendMessageToClient()
        {
            try
            {
                Console.Write("Msg:");
                string msg = Console.ReadLine();
                // send message to clients
                if (server.listClientsConnected.Count > 0)
                {
                    foreach (ClientInfo client in server.listClientsConnected)
                        server.sendMessage(msg, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sendMessageToClient();
            }
        }
    }
}
