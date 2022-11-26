using System.Windows.Input;

namespace ShotTest
{
    internal class Minoriko
    {
        Image image = Resource.Minoriko;
        Rectangle[,] trimRects = new Rectangle[4, 3];
        int time = 0, m, width, height, shootTime = 0;
        public Point position = new(225, 300);
        List<Shot> shots;

        public Minoriko(List<Shot> shots) 
        {
            width = image.Width / trimRects.GetLength(0);
            height = image.Height / trimRects.GetLength(1);
            for (int i = 0; i < trimRects.GetLength(0); i++) for (int j = 0; j < trimRects.GetLength(1); j++)
                    trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
            this.shots = shots;
        }

        public void Progress()
        {
            m = 0;
            if (Keyboard.IsKeyDown(Key.Right)) { position.X += 10; m = 1; }
            if (Keyboard.IsKeyDown(Key.Left)) { position.X -= 10; m = 2; }
            if (Keyboard.IsKeyDown(Key.Up)) position.Y -= 10;
            if (Keyboard.IsKeyDown(Key.Down)) position.Y += 10;
            if (++shootTime >= 3)
            {
                if (Keyboard.IsKeyDown(Key.Z))
                {
                    shots.Add(new Shot(new(position.X + 10, position.Y)));
                    shots.Add(new Shot(new(position.X - 10, position.Y)));
                }
                shootTime = 0;
            }
            time++;
        }

        public void Draw(Graphics graphics)
        {
            var n = (time / 10) % trimRects.GetLength(0);
            graphics.DrawImage(image, new Rectangle(-width / 2 + position.X, -height / 2 + position.Y, width, height), trimRects[n, m], GraphicsUnit.Pixel);
        }
    }
}
