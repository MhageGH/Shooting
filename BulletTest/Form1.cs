namespace BulletTest
{
    public partial class Form1 : Form
    {
        Minoriko minoriko;
        Shizuha shizuha;
        List<Shot> shots = new();
        List<Bullet> bullets = new();

        public Form1()
        {
            InitializeComponent();
            minoriko = new(shots);
            shizuha = new(minoriko, bullets);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            minoriko.Progress();
            shizuha.Progress();
            foreach (var shot in shots) shot.Progress();
            foreach (var bullet in bullets) bullet.Progress();
            shots.RemoveAll(shot => shot.enable == false);
            bullets.RemoveAll(bullet => bullet.enable == false);
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            minoriko.Draw(e.Graphics);
            shizuha.Draw(e.Graphics);
            foreach (var shot in shots) shot.Draw(e.Graphics);
            foreach (var bullet in bullets) bullet.Draw(e.Graphics);
        }
    }
}