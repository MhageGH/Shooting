using System.Numerics;

namespace RotateTest
{
    public partial class Form1 : Form
    {
        Image image = Properties.Resources.Maple;
        double theta = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            float x = image.Width / 2, y = image.Height / 2, c = (float)Math.Cos(theta), s = (float)Math.Sin(theta);
            var points = new Vector2[] { new(-x * c + y * s, -x * s - y * c), new(x * c + y * s, x * s - y * c), new(-x * c - y * s, -x * s + y * c) };
            var position = new Vector2(100, 100);
            e.Graphics.DrawImage(image, points.Select(x => (PointF)(x + position)).ToArray());
            int r = 5;
            e.Graphics.FillEllipse(Brushes.Black, position.X - r, position.Y - r, 2 * r, 2 * r);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            theta += 0.1;
            Invalidate();
        }
    }
}