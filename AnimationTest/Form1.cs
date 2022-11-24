namespace AnimationTest
{
    public partial class Form1 : Form
    {
        Image image = Resource.Minoriko;
        Rectangle[,] trimRects = new Rectangle[4, 3];
        int time = 0;

        public Form1()
        {
            InitializeComponent();
            int width = image.Width / trimRects.GetLength(0), height = image.Height / trimRects.GetLength(1);
            for (int i = 0; i < trimRects.GetLength(0); i++) for (int j = 0; j < trimRects.GetLength(1); j++)
                    trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var n = (time / 10) % trimRects.GetLength(0);
            var m = (time / 100) % trimRects.GetLength(1);
            e.Graphics.DrawImage(image, new Rectangle(50, 50, 100, 100), trimRects[n, m], GraphicsUnit.Pixel);
        }
    }
}