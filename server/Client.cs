using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    // tcp client
    public class Client
    {
        private Socket client { get; set; }
        private string clientName { get; set; }
        private string ip { get; set; }
        private int port { get; set; }

        // delegates
        public delegate void Client_Connected(ClientInfo c);
        public delegate void Client_Message(string msg, ClientInfo c);

        public Client(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        // connect client to server
        public void Connect()
        {
            ClientInfo clientInf = new ClientInfo()
            {
                currentClient = client,
            };
            client.BeginConnect(new IPEndPoint(IPAddress.Parse(this.ip), this.port), new AsyncCallback(ClientConnected), clientInf);
        }

        // accept client connection
        private void ClientConnected(IAsyncResult res)
        {
            ClientInfo clientInf = (ClientInfo)res.AsyncState;
            clientInf.currentClient.BeginReceive(clientInf.buffer, 0, ClientInfo.BUFFER_SIZE_FOR_MESSAGE, 0, new AsyncCallback(ReceiveMessage), clientInf);
            // client connected
            if (Cliente_Conneted_Event != null)
                Cliente_Conneted_Event(clientInf);
        }

        // receive message from client
        private void ReceiveMessage(IAsyncResult ar)
        {
            ClientInfo cl = (ClientInfo)ar.AsyncState;
            Socket s = cl.currentClient;
            int read = s.EndReceive(ar);
            string msg = null;
            if (read > 0)
            {
                msg = Encoding.ASCII.GetString(cl.buffer, 0, read);
                s.BeginReceive(cl.buffer, 0, ClientInfo.BUFFER_SIZE_FOR_MESSAGE, 0, new AsyncCallback(ReceiveMessage), cl);
            }
            if (Cliente_Message_Event != null)
                Cliente_Message_Event(msg, cl);
        }

        // send message to client
        public void sendMessage(string msg, ClientInfo client)
        {
            if (client.currentClient != null)
            {
                if (client.currentClient.Connected && !string.IsNullOrEmpty(msg))
                {
                    client.currentClient.Send(Encoding.ASCII.GetBytes(msg));
                }
            }
        }

        public event Client_Connected Cliente_Conneted_Event;
        public event Client_Message Cliente_Message_Event;

    }
}
