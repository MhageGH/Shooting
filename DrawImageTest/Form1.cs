using System.Numerics;

namespace DrawImageTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Image image = Resource.BigMinoriko;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawImage(image, new Point[] { new(200, 50), new(400, 50), new(200, 450) });
            //e.Graphics.DrawImage(image, new Point[] { new(38, 179), new(179, 38), new(321, 462) });
            e.Graphics.DrawImage(image, new Point[] { new(100, 50), new(400, 50), new(100, 350) },
                new Rectangle(50, 50, 150, 150), GraphicsUnit.Pixel);
        }
    }
}