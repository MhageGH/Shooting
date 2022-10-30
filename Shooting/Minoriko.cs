using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Shooting
{
    internal class Minoriko
    {
        static Point initial_position = new Point(BackGround.position.X + BackGround.screen_size.Width / 2, BackGround.position.Y + BackGround.screen_size.Height);
        static int initial_life = 3;
        static float high_speed = 4;
        static float low_speed = 2;
        static Image image = Properties.Resources.Minoriko;
        static Rectangle[,] trimRects = new Rectangle[4, 3];
        readonly int width = image.Width / trimRects.GetLength(0);
        readonly int height = image.Height / trimRects.GetLength(1);
        readonly int animInterval = 10;
        int time = 0;
        (int x, int y) trimNumber;

        public PointF position = new (initial_position.X, initial_position.Y - 100);

        public Minoriko()
        {
            for (int i = 0; i < trimRects.GetLength(0); i++)
            {
                for (int j = 0; j < trimRects.GetLength(1); j++)
                {
                    trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
                }
            }
        }

        public void Progress()
        {
            trimNumber.y = 0;
            time = (time + 1) % (animInterval * trimRects.GetLength(0));
            float speed = high_speed;
            if (Keyboard.IsKeyDown(Key.LeftShift)) speed = low_speed;   // プロジェクトのプロパティでWPFを有効にすることでKeyboardクラスが使える
            if (Keyboard.IsKeyDown(Key.Right))
            {
                position.X += speed;
                trimNumber.y = 1;
            }
            if (Keyboard.IsKeyDown(Key.Left))
            {
                position.X -= speed;
                trimNumber.y = 2;
            }
            if (Keyboard.IsKeyDown(Key.Up)) position.Y -= speed;
            if (Keyboard.IsKeyDown(Key.Down)) position.Y += speed;
        }

        public void Draw(Graphics graphics)
        {
            trimNumber.x = time / animInterval;
            graphics.DrawImage(image, new Rectangle((int)position.X, (int)position.Y, width, height), trimRects[trimNumber.x, trimNumber.y], GraphicsUnit.Pixel);
        }
    }
}
