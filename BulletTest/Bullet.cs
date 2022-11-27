using System.Numerics;

namespace BulletTest
{
    internal class Bullet
    {
        static Image[] images = new Bitmap[] { Resource.Bullet0, Resource.Bullet1, Resource.Bullet2, Resource.Bullet3,
                                                Resource.Bullet4, Resource.Bullet5, Resource.Bullet6, Resource.Bullet7 };
        static float[] radii = new float[] { 4, 2, 3, 13, 3, 3, 2, 2 };
        int imageID = 0, actID = 0;
        bool rotatable = false;     // 速度の方向に向くように回転するか否か
        Vector2 speed = new();
        public Vector2 position = new();
        public bool enable = true;
        public readonly float radius;

        public Bullet(int imageID, int actID, bool rotatable, Vector2 position, Vector2 speed)
        {
            this.imageID = imageID;
            this.actID = actID;
            this.rotatable = rotatable;
            this.position = position;
            this.speed = speed;
            this.radius = radii[imageID];
        }

        public void Progress()
        {
            if (actID == 1) speed = (speed.Length() + 0.03f) * (speed / speed.Length());    // 等加速度直線運動
            else if (actID == 2) speed.Y += 0.15f;                                          // 重力加速度運動
            position += speed;
            if (position.X < 0 || position.X > 450 || position.Y < 0 || position.Y > 489) enable = false;
        }

        public void Draw(Graphics graphics)
        {
            var image = images[imageID];
            if (rotatable)
            {
                var angle = -MathF.Atan2(speed.X, speed.Y);
                float x = image.Width / 2, y = image.Height / 2, c = MathF.Cos(angle), s = MathF.Sin(angle);
                var points = new Vector2[] { new(-x * c + y * s, -x * s - y * c), new(x * c + y * s, x * s - y * c), new(-x * c - y * s, -x * s + y * c) };
                graphics.DrawImage(image, points.Select(point => (PointF)(point + position)).ToArray());
            }
            else graphics.DrawImage(image, position.X - image.Width / 2, position.Y - image.Height / 2, image.Width, image.Height);
        }
    }
}