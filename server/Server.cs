using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public class Server
    {
        private string ip { get; set; }
        private int port { get; set; }
        private Socket server { get; set; }
        public List<ClientInfo> listClientsConnected { get; private set; }

        // delegate for events
        public delegate void Client_Connected(ClientInfo c);
        public delegate void Client_Message(string msg, ClientInfo c);
        public Server(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.listClientsConnected = new List<ClientInfo>();
        }

        // start server
        public void Start()
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse(this.ip), this.port));
            server.Listen(1);
            server.BeginAccept(new AsyncCallback(ClientConnected), server);
        }

        // accept client connection
        private void ClientConnected(IAsyncResult res)
        {
            Socket s = (Socket)res.AsyncState;
            Socket client = s.EndAccept(res);
            ClientInfo clientInf = new ClientInfo()
            {
                currentClient = client
            };

            client.BeginReceive(clientInf.buffer, 0, ClientInfo.BUFFER_SIZE_FOR_MESSAGE, 0, new AsyncCallback(ReceiveMessage), clientInf);
            // add client to list
            listClientsConnected.Add(clientInf);
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

        //events
        public event Client_Connected Cliente_Conneted_Event;
        public event Client_Message Cliente_Message_Event;


    }
}
