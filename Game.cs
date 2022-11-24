using System.Timers;
using System.Net;
using System.Text;
using System.Net.Sockets;
namespace SpaceGame
{
    public partial class Game : Form
    {

        //  private static System.Timers.Timer aTimer;
        //stored all steps: index is time, the first value is x and second value is y
        List<step> steps = new List<step>();
        // position of this color
        PointF p = new PointF(125, 100);
        PointF p2 = new PointF(377, 100);
        TimeSpan ts;
        private Socket _socket;
        private static byte[] result = new byte[1024];

        public Game(Socket _socket)
        {
           this._socket = _socket;  
            InitializeComponent();
            //connect to server for start game
            aTimer.Enabled = false;
            DialogResult dialogResult = MessageBox.Show("sei pronto per giocare ?", "Conferma",MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes )
            {
                // Set KeyPreview object to true to allow the form to process
                // the key before the control with focus processes it.
                if (this._socket != null)
                {
                 //   SentMsg(buildMsg(1, "type of game"), this._socket);
                    KeyPreview = true;
                    aTimer.Enabled = true;
                    // Associate the event-handling method with the KeyDown event
                    this.KeyDown += new KeyEventHandler(Form1_KeyDown);
                    SentMsg(buildMsg(2,"ready"), _socket);
                    Thread myThread = new Thread(ListenClientConnect);
                    myThread.Start();
                }
                else
                {
                    MessageBox.Show("no connect");
                    this.Close();
                }
              
            }
            else if (dialogResult == DialogResult.No)
                this.Close();
               
          
        }
        
        public Socket MySocket
        {
            get { return _socket; }
            set { _socket = value; }
        }
        private void SentMsg(string sentstr, Socket clientSocket)
        {
            string sendMessage = sentstr;//the strign send for server
            sendMessage = sentstr;//context send for server
            clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage));
        }


        //listen message 
        private void ListenClientConnect()
        {
            while (true)
            {
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(_socket);
            }
        }
        private void ReceiveMessage(object clientSocket)
        {
            string sendStr;
            Socket myClientSocket =(Socket) clientSocket;
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
                    //check this name is validate
                    sendStr = checkTypeMsg(risultato);

                }
                catch (Exception ex)
                {
                  //  myClientSocket.Close();//close clientsoket
                    break;
                }
            }
        }

        private string checkTypeMsg(string msg)
        {
            if (msg != "" && msg != null)
            {
                string[] str = msg.Split(';');// [0] type  [1] context
                int type = Int32.Parse(str[0]);
                string context=str[1];
                switch (type)
                {
                    case 2:
                        int passage=Int32.Parse(context);
                        switch (passage)
                        {
                            case 0:
                                p2.X -= 10;
                                break;
                            case 1:
                                p2.X += 10;
                                break;
                            case 2:
                                p2.Y -= 10;
                                break;
                            case 3:
                                p2.Y += 10;
                                break;
                        }
                        break;
                    case 3:

                        break;
                    default:
                        break;
                }
            }
            Invalidate();
            return buildMsg(0, "false");

        }
        private string buildMsg(int type, string msg)
        {
            return type + ";" + msg;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
           
            //check the bord of map
            switch (e.KeyCode)
            {
                case Keys.Left:
                    p.X -= 10;
                    break;
                case Keys.Right:
                    p.X += 10;
                    break;
                case Keys.Up:
                    p.Y -= 10;
                    break;
                case Keys.Down:
                    p.Y += 10;
                    break;
                default:
                    break;
            }
            ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            SentMsg(buildMsg(2, steps.ToString()),_socket);
            steps.Add(new step(Convert.ToInt64(ts.TotalSeconds), p.X,p.Y));
            //if is not key need,the key become be invalidated
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //take map 
            System.Drawing.Bitmap bmp = new Bitmap(pictureBox1.Image);
            //draw circle
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(new SolidBrush(Color.Red), new RectangleF(p, new SizeF(10, 10)));
            g.FillEllipse(new SolidBrush(Color.Blue), new RectangleF(p2, new SizeF(10, 10)));
            //put image drawed 
            pictureBox1.Image = bmp;
        }

        //connect to server, now it used simulate player alone so use writeMove
        private void writeMove()
        {
            using StreamWriter file = new("steps.txt");
            foreach (step s in steps)
            {
                file.Write(s.tostring()+"\n");
            }
        }
        private void aTimer_Tick(object sender, EventArgs e)
        {
            LabelTime.Text = ((Int32.Parse(LabelTime.Text) + 1).ToString());
            if (LabelTime.Text == "5")
            {
                writeMove();
                aTimer.Stop();
                this.Close();
            }
        }
    }
}