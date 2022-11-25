using System.Windows.Input;

namespace KeyInputTest
{
    public partial class Form1 : Form
    {
        Image image = Resource.Minoriko;
        Rectangle[,] trimRects = new Rectangle[4, 3];
        int time = 0, m;
        Point position = new(100, 100);

        public Form1()
        {
            int width = image.Width / trimRects.GetLength(0), height = image.Height / trimRects.GetLength(1);
            for (int i = 0; i < trimRects.GetLength(0); i++) for (int j = 0; j < trimRects.GetLength(1); j++)
                    trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m = 0;
            if (Keyboard.IsKeyDown(Key.Right)) { position.X += 10; m = 1; }
            if (Keyboard.IsKeyDown(Key.Left)) { position.X -= 10; m = 2; }
            if (Keyboard.IsKeyDown(Key.Up)) position.Y -= 10;
            if (Keyboard.IsKeyDown(Key.Down)) position.Y += 10;
            time++;
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var n = (time / 10) % trimRects.GetLength(0);
            e.Graphics.DrawImage(image, new Rectangle(position.X, position.Y, 100, 100), trimRects[n, m], GraphicsUnit.Pixel);
        }
    }
}