using System.Windows.Input;

namespace Shooting
{
    internal class Minoriko
    {
        readonly static Point initial_position = new Point(BackGround.position.X + BackGround.screen_size.Width / 2, BackGround.position.Y + BackGround.screen_size.Height);
        readonly static int initial_life = 3;
        readonly static float high_speed = 4;
        readonly static float low_speed = 2;
        Image image = Properties.Resources.Minoriko;
        int animationTime = 0;

        public PointF position = new (initial_position.X, initial_position.Y - 100);

        public void Progress()
        {
            animationTime = (animationTime + 1) % 40;
            float speed = high_speed;
            if (Keyboard.IsKeyDown(Key.LeftShift)) speed = low_speed;   // プロジェクトのプロパティでWPFを有効にすることでKeyboardクラスが使える
            if (Keyboard.IsKeyDown(Key.Right)) position.X += speed;
            if (Keyboard.IsKeyDown(Key.Left)) position.X -= speed;
            if (Keyboard.IsKeyDown(Key.Up)) position.Y -= speed;
            if (Keyboard.IsKeyDown(Key.Down)) position.Y += speed;
        }

        public void Draw(Graphics graphics)
        {
            int n = animationTime / 10;
            graphics.DrawImage(image, new Rectangle((int)position.X, (int)position.Y, 50, 50), new Rectangle(50 * n, 0, 50, 50), GraphicsUnit.Pixel);
        }

    }
}
