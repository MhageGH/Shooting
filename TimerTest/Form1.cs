namespace TimerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int x = 0;
        Image image = Resource.BigMinoriko;

        private void timer1_Tick(object sender, EventArgs e)
        {
            x += 10;
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(image, new Point[] { new(x, 0), new(x + 100, 0), new(x, 200) });
        }
    }
}