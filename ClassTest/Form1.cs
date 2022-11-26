namespace ClassTest
{
    public partial class Form1 : Form
    {
        Minoriko minoriko = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            minoriko.Progress();
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            minoriko.Draw(e.Graphics);
        }
    }
}