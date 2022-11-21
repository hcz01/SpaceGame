using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SpaceGame
{
    public partial class main : Form
    {
        regist regist = new regist();// the window can regist this client name
        string Message;
        Socket clientSocket;
        public main()
        {
            InitializeComponent();
           
           
        }


        public void longlink()
        {
            //set ip and port
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 2000)); //set soket with ip and port 
                listBox1.Items.Add("connessione server successo");
            }
            catch
            {
                listBox1.Items.Add("connessione server failed. riporvare con il tasto enter");
                return;
            }

            // the first conversation is add this client name on server
                string sentstr= registClientName();
               
            

        }



        public void SentMsg(string sentstr, Socket clientSocket)
        {
            string sendMessage = "hi";//the strign send for server
            sendMessage = sentstr;//context send for server
            clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage));
            listBox1.Items.Add("il messaggio che hai mandato al server：" + sendMessage);
            //accept the message from server 
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //Receive messages from the server 
            recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);
            listBox1.Items.Add("il messaggio riceve dal server：{0}" + " " + recvStr);    //show the message 
            
        }

        //the method is used to see if the name is correct
        public string CheckName(string sentstr, Socket clientSocket)
        {
            string sendMessage = sentstr;//string send for server
            //send name
                clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage));
                    listBox1.Items.Add("ii nome che hai mandato：" + sendMessage);
            //accept the message from server 
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
                bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //receive the message from server 
                recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);
            if(recvStr!="false")
                    listBox1.Items.Add("il nome di questo client e' " + recvStr);    //show the message 

            return recvStr;
        }


        //method is used to regist name
       private string registClientName()
        {
            string sentstr;
            do
            {
                regist.ShowDialog();
                sentstr = regist.name;

                if (sentstr != "" && CheckName(sentstr, clientSocket) !="false")
                    break;

            } while (true);
            regist.Dispose();
            return sentstr;
        }

        private void ConnetServer_Click(object sender, EventArgs e)
        {
            longlink();
        }
    }
}
