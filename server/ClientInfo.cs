using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    // class for client info
    public class ClientInfo
    {
        public Socket currentClient = null;
        public const int BUFFER_SIZE_FOR_MESSAGE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE_FOR_MESSAGE];
    }
}
