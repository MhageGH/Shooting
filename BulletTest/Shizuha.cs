using System.Numerics;

namespace BulletTest
{
    internal class Shizuha
    {
        Image image = Resource.Shizuha;
        Rectangle[,] trimRects = new Rectangle[4, 3];
        int width, height, interval = 10, time0 = 0, time1, time2, time3, endOfTime = 0;
        Minoriko minoriko;
        Vector2 position = new(200, 125), speed = new(0, 0);
        List<Bullet> bullets;

        public Shizuha(Minoriko minoriko, List<Bullet> bullets)
        {
            this.minoriko = minoriko; this.bullets = bullets;
            width = image.Width / trimRects.GetLength(0); height = image.Height / trimRects.GetLength(1);
            for (int i = 0; i < trimRects.GetLength(0); i++) for (int j = 0; j < trimRects.GetLength(1); j++)
                    trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
        }

        public void Progress()
        {
            if (time1 == 0)
            {
                Vector2 direction = new();
                Random rand = new();
                endOfTime = (int)(15 * rand.NextSingle() + 10);
                float theta = (MathF.PI / 2) * (rand.NextSingle() - 0.5f), cos = MathF.Cos(theta), sin = MathF.Sin(theta), asin = MathF.Abs(sin);
                direction.X = ((minoriko.position.X - position.X >= 0 && position.X < 350) || position.X < 50) ? cos : -cos;
                direction.Y = (position.Y < 100) ? asin : (position.Y > 150) ? -asin : sin;
                speed = 4 * direction;
            }
            else if (time1 < endOfTime) position += speed;
            else speed = new(0, 0);
            if (++time1 >= 45) time1 = 0;
            time0 = (time0 + 1) % (interval * trimRects.GetLength(0));
            if (time2 == 0) time3 = 0;
            if (50 < time2 && time2 % 150 < 100) for (int i = 0; i < 3; ++i) BulletMake(position, minoriko.position, i);
            time2++; time3++;
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(image, new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height),
                trimRects[time0 / interval, speed.X > 0 ? 1 : speed.X < 0 ? 2 : 0], GraphicsUnit.Pixel);
        }

        void BulletMake(Vector2 source_position, Vector2 target_position, int ID)
        {
            switch (ID)
            {
                case 0: // 3-way 固定
                    if (time3 % 10 == 0)
                    {
                        var dirs = new Vector2[] { new(0, 1), new(0.5f, 0.866f), new(-0.5f, 0.866f) };
                        for (int i = 0; i < dirs.Length; ++i)
                        {
                            var position = source_position + 30 * dirs[i];
                            var speed = 4 * dirs[i];
                            bullets.Add(new(0, 0, false, position, speed));
                        }
                    }
                    break;
                case 1:　//自機狙い×3
                    if (time3 % 2 == 0)
                    {
                        var offsets = new Vector2[] { new(0, 50), new(-50, 50), new(50, 50) };
                        foreach (var offset in offsets)
                        {
                            var position = source_position + offset;
                            var v = target_position - position;
                            var speed = 10 * v / v.Length();
                            bullets.Add(new(1, 0, true, position, speed));
                        }
                    }
                    break;
                case 2: // 放射状固定弾×4。一定速度で発射方向回転
                    if (time3 % 3 == 0)
                    {
                        const float pi = MathF.PI;
                        var angles = new float[] { pi * 5 / 12 + pi * time3 / 50, -pi * 5 / 12 - pi * time3 / 50, pi * 7 / 12 - pi * time3 / 50, -pi * 7 / 12 + pi * time3 / 50 };
                        foreach (var angle in angles)
                        {
                            var dir = new Vector2(MathF.Sin(angle), MathF.Cos(angle));
                            var position = source_position + 30 * dir;
                            var speed = 8 * dir;
                            bullets.Add(new(2, 0, true, position, speed));
                        }
                    }
                    break;
            }
        }
    }
}
