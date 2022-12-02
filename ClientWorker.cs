using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame
{
    public class ClientWorker
    {
        private Socket clientSocket;
        private main form;
        public ClientWorker(main form, Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            this.form = form;
        }

        public void doClient()
        {
            form.ReceiveMessage(clientSocket);
        }
    }
}