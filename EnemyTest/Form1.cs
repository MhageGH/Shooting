namespace EnemyTest
{
    public partial class Form1 : Form
    {
        Minoriko minoriko;
        Shizuha shizuha;

        public Form1()
        {
            InitializeComponent();
            minoriko = new();
            shizuha = new(minoriko);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            minoriko.Progress();
            shizuha.Progress();
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            minoriko.Draw(e.Graphics);
            shizuha.Draw(e.Graphics);
        }
    }
}