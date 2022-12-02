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
        Socket clientSocket;
        private static byte[] result = new byte[1024];
        DialogResult dialogResult;
        public main()
        {
            InitializeComponent();

        }



        private void longlink()
        {
            //set ip and port
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 2000)); //set soket with ip and port 
                listBox1.Items.Add("connessione server successo");
                ConnetServer.Enabled = false;
            }
            catch
            {
                listBox1.Items.Add("connessione server failed.");
                return;
            }

            // the first conversation is add this client name on server
            string sentstr = registClientName();
        }


        private string SentMsg(string sentstr, Socket clientSocket)
        {
            string sendMessage = sentstr;//the strign send for server
            sendMessage = sentstr;//context send for server
            clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage));
            listBox1.Items.Add("il messaggio che hai mandato al server：" + sendMessage);

            //accept the message from server 
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //Receive messages from the server 
            recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);
            string str = checkTypeMsg(recvStr);
            listBox1.Items.Add("il messaggio riceve dal server：{0}" + " " + recvStr);    //show the message 
            return str;

        }
        private string checkTypeMsg(string msg)
        {
            if (msg != "" && msg != null)
            {
                string[] str = msg.Split(';');// [0] type  [1] context
                int type = Int32.Parse(str[0]);
                string context = str[1];
                switch (type)
                {
                    // regist name
                    case 0:
                        if (str[1] == "true")
                        {
                            listBox1.Items.Add("registra il nome successo");    //show the message 
                            return buildMsg(0, "true");
                        }

                        break;

                    case 4:
                        if (context == "ready")
                            return buildMsg(4, "true");
                        else
                            return buildMsg(4, "false");

                        break;

                    default:
                        break;
                }
            }
            return buildMsg(0, "false");

        }

        //method is used to regist name
        private string registClientName()
        {
            string sentstr;
            do
            {
                regist.ShowDialog();
                sentstr = buildMsg(0, regist.name);
                if (sentstr != "" && SentMsg(sentstr, clientSocket).Split(";")[1] != "false")
                    break;

            } while (true);
            //  regist.Dispose();
            ClientName.Text = regist.name;
            return sentstr;
        }


        private void ConnetServer_Click(object sender, EventArgs e)
        {
            longlink();
            button1.Enabled = true;
            button2.Enabled = true;


        }
        private string buildMsg(int type, string msg)
        {
            return type + ";" + msg;
        }

        private void ListenClientConnect()
        {
            dialogResult = MessageBox.Show("stai aspettando altri giocatore ", "alert", MessageBoxButtons.OK);
            if (dialogResult == DialogResult.Yes)
            {
                ClientWorker cw = new ClientWorker(this, clientSocket);
                Thread receiveThread = new Thread(cw.doClient);
                receiveThread.Start(clientSocket);
            }
        }
        public void ReceiveMessage(object clientSocket)
        {

            string sendStr;
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {

                    //use SocketClient for receive the result
                    int receiveNumber = myClientSocket.Receive(result);
                    if (receiveNumber == 0)
                        return;
                    string risultato = Encoding.UTF8.GetString(result, 0, receiveNumber);
                    //return the data of client
                    sendStr = checkTypeMsg(risultato);
                    if (sendStr == buildMsg(4, "true"))
                    {
                        dialogResult = DialogResult.Yes;
                        wait2pk();
                    }
                }
                catch (Exception ex)
                {
                    //  myClientSocket.Close();//close clientsoket
                    break;
                }
            }
        }
        //vs pc
        private void button2_Click(object sender, EventArgs e)
        {
            Game g = new Game(clientSocket);
            if (g != null && !g.IsDisposed)
                g.Show();
        }
        //vs client
        private void button1_Click(object sender, EventArgs e)
        {
            string msg = SentMsg(buildMsg(4, ClientName.Text), clientSocket);

            if (msg == buildMsg(4, "true"))
            {
                wait2pk();
            }
            else
                ListenClientConnect();
        }

        private void wait2pk()
        {
            Game g = new Game(clientSocket);
            if (g != null && !g.IsDisposed)
                g.Show();
        }
    }
}
