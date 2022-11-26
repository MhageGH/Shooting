using System.Numerics;

namespace ShotTest
{
    internal class Shizuha
    {
        Image image = Resource.Shizuha;
        Rectangle[,] trimRects = new Rectangle[4, 3];
        int width, height, interval = 10, time_anim = 0, time_move, endOfTime = 0;
        Minoriko minoriko;
        Vector2 position = new(200, 125), speed = new(0, 0);

        public Shizuha(Minoriko minoriko)
        {
            this.minoriko = minoriko;
            width = image.Width / trimRects.GetLength(0);
            height = image.Height / trimRects.GetLength(1);
            for (int i = 0; i < trimRects.GetLength(0); i++) for (int j = 0; j < trimRects.GetLength(1); j++)
                    trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
        }

        public void Progress()
        {
            if (time_move == 0)
            {
                Vector2 direction = new();
                Random rand = new();
                endOfTime = (int)(15 * rand.NextSingle() + 10);
                float theta = (MathF.PI / 2) * (rand.NextSingle() - 0.5f), cos = MathF.Cos(theta), sin = MathF.Sin(theta), asin = MathF.Abs(sin);
                direction.X = ((minoriko.position.X - position.X >= 0 && position.X < 350) || position.X < 50) ? cos : -cos;
                direction.Y = (position.Y < 100) ? asin : (position.Y > 150) ? -asin : sin;
                speed = 4 * direction;
            }
            else if (time_move < endOfTime) position += speed;
            else speed = new(0, 0);
            if (++time_move >= 45) time_move = 0;
            time_anim = (time_anim + 1) % (interval * trimRects.GetLength(0));
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(image, new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height),
                trimRects[time_anim / interval, speed.X > 0 ? 1 : speed.X < 0 ? 2 : 0], GraphicsUnit.Pixel);
        }
    }
}
