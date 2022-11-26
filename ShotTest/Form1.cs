namespace ShotTest
{
    public partial class Form1 : Form
    {
        Minoriko minoriko;
        Shizuha shizuha;
        List<Shot> shots = new();

        public Form1()
        {
            InitializeComponent();
            minoriko = new(shots);
            shizuha = new(minoriko);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            minoriko.Progress();
            shizuha.Progress();
            foreach (var shot in shots) shot.Progress();
            shots.RemoveAll(shot => shot.enable == false);
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            minoriko.Draw(e.Graphics);
            shizuha.Draw(e.Graphics);
            foreach (var shot in shots) shot.Draw(e.Graphics);
        }
    }
}