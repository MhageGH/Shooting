using System.Numerics;

namespace ScaleUpDownTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Image image = Resource.BigMinoriko;
        bool scaleUp = true;
        float scale = 1;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            float x = image.Width / 2, y = image.Height / 2;
            var points = new Vector2[] { new(-x, -y), new(x, -y), new(-x, y) };
            var position = new Vector2(220, 260);
            e.Graphics.DrawImage(image, points.Select(x => (PointF)(scale * x + position)).ToArray());

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            scale = scaleUp ? scale * 1.05f : scale * 0.95f;
            scaleUp = scale > 1.2f ? false : scale < 0.5f ? true : scaleUp;
            Invalidate();
        }
    }
}