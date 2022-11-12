namespace SpaceGame
{
    public partial class Form1 : Form
    {
        PointF p = new PointF(100, 100);
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

    }
}