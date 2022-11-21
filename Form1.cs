using System.Timers;
using System.Net;
using System.Text;
using System.Net.Sockets;
namespace SpaceGame
{
    public partial class Form1 : Form
    {

        //  private static System.Timers.Timer aTimer;
        //stored all steps: index is time, the first value is x and second value is y
        List<step> steps = new List<step>();
        // position of this color
        PointF p = new PointF(100, 100);


        TimeSpan ts;
        public Form1()
        {
            InitializeComponent();
            // Set KeyPreview object to true to allow the form to process
            // the key before the control with focus processes it.
            KeyPreview = true;
            // Associate the event-handling method with the
            // KeyDown event.
             this.KeyDown += new KeyEventHandler(Form1_KeyDown);
     



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