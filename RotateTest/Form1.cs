using System.Numerics;

namespace RotateTest
{
    public partial class Form1 : Form
    {
        Image image = Properties.Resources.BigMinoriko;
        float theta = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            float x = image.Width / 2, y = image.Height / 2, c = MathF.Cos(theta), s = MathF.Sin(theta);
            var points = new Vector2[] { new(-x * c + y * s, -x * s - y * c), new(x * c + y * s, x * s - y * c), new(-x * c - y * s, -x * s + y * c) };
            var position = new Vector2(220, 220);
            e.Graphics.DrawImage(image, points.Select(x => (PointF)(x + position)).ToArray());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            theta += 0.1f;
            Invalidate();
        }
    }
}