using System.Timers;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;

namespace SpaceGame
{
    public partial class Game : Form
    {

        //  private static System.Timers.Timer aTimer;
        //stored all steps: index is time, the first value is x and second value is y
        List<step> steps;
        List<step> steps2;

        List<step> copy_steps;
        List<step> copy_steps2;
        // position of this color
        PointF p;
        PointF p2;
        TimeSpan ts;
        private Socket _socket;
        private static byte[] result;

        public Game(Socket _socket)
        {
            this._socket = _socket;
            InitializeComponent();

            steps = new List<step>();
            steps2 = new List<step>();

            copy_steps = new List<step>();
            copy_steps2 = new List<step>();

            result = new byte[1024];

            // position of this color
            p = new PointF(305, 100);
            p2 = new PointF(375, 100);

            //connect to server for start game
            aTimer.Enabled = false;
            DialogResult dialogResult = MessageBox.Show("sei pronto a giocare ?", "Conferma", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Set KeyPreview object to true to allow the form to process
                // the key before the control with focus processes it.
                if (this._socket != null)
                {
                    KeyPreview = true;
                    aTimer.Enabled = true;
                    // Associate the event-handling method with the KeyDown event
                    this.KeyDown += new KeyEventHandler(Form1_KeyDown);
                    //init steps stored
                    ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    steps.Add(new step(Convert.ToInt64(ts.TotalSeconds), p.X, p.Y));
                    steps2.Add(new step(Convert.ToInt64(ts.TotalSeconds), p2.X, p2.Y));
                    copy_steps.Add(new step(Convert.ToInt64(ts.TotalSeconds), p.X, p.Y));
                    copy_steps2.Add(new step(Convert.ToInt64(ts.TotalSeconds), p2.X, p2.Y));
                    // the first messaage for communicate server to start game
                    SentMsg(buildMsg(2, "ready"), _socket);
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
        //send message
        private void SentMsg(string sentstr, Socket clientSocket)
        {
            string sendMessage = sentstr;//the strign send for server
            sendMessage = sentstr;//context send for server
            if (sendMessage != null && sendMessage.Length > 0)
            {
                clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage));
            }

        }

        //listen message 
        private void ListenClientConnect()
        {
            ReceiveMessage(_socket);

        }
        private void ReceiveMessage(object clientSocket)
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

                    //Debug.WriteLine("ricevuto: "+risultato);
                    //return the data of client


                    sendStr = checkTypeMsg(risultato);

                }
                catch (Exception ex)
                {
                    //  Debug.WriteLine(ex.ToString());
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
                if (str.Length == 0) return "";

                Debug.WriteLine("Pos0: " + str[0]);
                if (str[0] == "False") return "";

                int type = Int32.Parse(str[0]);
                int passage;
                string context = str[1];
                if (context.Length == 0) return "";
                switch (type)
                {
                    case 2:
                        Debug.WriteLine("Pos1: " + context);
                        if (context == "true0" || context == "true") return "";
                        passage = Int32.Parse(context);
                        float x = p2.X;
                        float y = p2.Y;
                        switch (passage)
                        {
                            case 0:
                                x -= 10;
                                break;
                            case 1:
                                x += 10;
                                break;
                            case 2:
                                y -= 10;
                                break;
                            case 3:
                                y += 10;
                                break;
                        }
                        bool isV = Pvalidate(ref x, ref y);
                        p2.X = x;
                        p2.Y = y;
                        ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        if (isV != false)
                        {
                            steps2.Add(new step(Convert.ToInt64(ts.TotalSeconds), p2.X, p2.Y));
                            copy_steps2.Add(new step(Convert.ToInt64(ts.TotalSeconds), p2.X, p2.Y));
                        }
                        Invalidate();
                        break;
                    case 3:


                        break;
                    default:
                        break;
                }
            }

            return buildMsg(0, "false");

        }
        private string buildMsg(int type, string msg)
        {
            return type + ";" + msg;
        }

        //draw moviment
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            string str;
            float x = p.X;
            float y = p.Y;
            //check the bord of map
            switch (e.KeyCode)
            {

                case Keys.Left:
                    x -= 10;
                    break;

                case Keys.Right:
                    x += 10;
                    break;

                case Keys.Up:
                    y -= 10;
                    break;

                case Keys.Down:
                    y += 10;
                    break;
                default:
                    break;
            }

            bool isV = Pvalidate(ref x, ref y);
            p.X = x;
            p.Y = y;
            str = p.X.ToString() + ";" + p.Y.ToString();
            ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            if (isV != false)
            {
                steps.Add(new step(Convert.ToInt64(ts.TotalSeconds), p.X, p.Y));
                copy_steps.Add(new step(Convert.ToInt64(ts.TotalSeconds), p.X, p.Y));
            }


            SentMsg(buildMsg(3, str), _socket);
            Invalidate();

        }
        //check does is position validate
        private bool Pvalidate(ref float x, ref float y)
        {

            if (x >= 490)
                x -= 10;
            else if (x <= 0)
                x += 10;

            if (y >= 350)
                y -= 10;
            else if (y <= 0)
                y += 10;

            if ((x == p.X && y == p.Y) || (x == p2.X && y == p2.Y))
                return false;

            return true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //take map 
            System.Drawing.Bitmap bmp = new Bitmap(pictureBox1.Image);
            //draw circle
            Graphics g = Graphics.FromImage(bmp);
            PointF pp = p;
            PointF pp2 = p2;
            if (copy_steps.Count > 1)
                pp = new PointF(copy_steps[copy_steps.Count - 2].x, copy_steps[copy_steps.Count - 2].y);


            if (copy_steps2.Count > 1)
            {
                pp2 = new PointF(copy_steps2[copy_steps2.Count - 2].x, copy_steps2[copy_steps2.Count - 2].y);
                copy_steps2.RemoveAt(copy_steps2.Count - 2);
            }
            //server
            g.FillEllipse(new SolidBrush(Color.Green), new RectangleF(pp2, new SizeF(10, 10)));
            g.FillEllipse(new SolidBrush(Color.Blue), new RectangleF(p2, new SizeF(10, 10)));
            //client
            g.FillEllipse(new SolidBrush(Color.Red), new RectangleF(pp, new SizeF(10, 10)));
            g.FillEllipse(new SolidBrush(Color.Black), new RectangleF(p, new SizeF(10, 10)));

            //put image drawed 
            pictureBox1.Image = bmp;
        }

        //connect to server, now it used simulate player alone so use writeMove

        private void aTimer_Tick(object sender, EventArgs e)
        {
            LabelTime.Text = ((Int32.Parse(LabelTime.Text) + 1).ToString());
            if (LabelTime.Text == "10")
            {
                aTimer.Enabled = false;
                int n = Winner();
                switch (n)
                {
                    case 0:
                        MessageBox.Show("pareggio");
                        break;
                    case 1:
                        MessageBox.Show("tu hai vinto");
                        break;
                    case 2:
                        MessageBox.Show("pc ha vinto");
                        break;

                }
                aTimer.Stop();

                this.Close();
            }
        }
        private int Winner()
        {
            string str = "";
            //descending
            steps.Sort((a, b) => a.CompareTo(b));
            steps2.Sort((a, b) => a.CompareTo(b));
            //take only position has been once with time max
            List<step> stepDistinct = steps.Where((a, b) => steps.FindIndex(c => (c.x == a.x && c.y == a.y)) == b).ToList();
            List<step> step2Distinct = steps2.Where((a, b) => steps2.FindIndex(c => (c.x == a.x && c.y == a.y)) == b).ToList();
            //finde items with same position
            stepDistinct.Sort((a, b) => a.CompareTo(b));
            step2Distinct.Sort((a, b) => a.CompareTo(b));
            var exp1 = stepDistinct
                .Where(a => step2Distinct.Any(t => (a.x == t.x && a.y == t.y)))
                .ToList();

            //Remove 2 identical positions and times less than the largest (exp1)
            foreach (step item in stepDistinct.ToList())
            {
                foreach (step item2 in exp1)
                {
                    if (item2.x == item.x && item2.y == item.y && item.sec < item2.sec)
                        stepDistinct.Remove(item);
                }

            }
            foreach (step item in step2Distinct.ToList())
            {
                foreach (step item2 in exp1)
                {

                    if (item2.x == item.x && item2.y == item.y && item.sec < item2.sec)
                        step2Distinct.Remove(item);
                }
            }


            if (stepDistinct.Count == step2Distinct.Count)
                return 0;
            else if (stepDistinct.Count > step2Distinct.Count)
                return 1;
            else
                return 2;

            return -1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}