using System.Numerics;
using System.Windows;

namespace Shooting
{
    /// <summary>
    /// 敵弾クラス
    /// </summary>
    internal class Bullet : ShootingObject
    {
        static Image[] images = new Bitmap[]
        {
            Properties.Resources.Bullet0, Properties.Resources.Bullet1, Properties.Resources.Bullet2, Properties.Resources.Bullet3,
            Properties.Resources.Bullet4, Properties.Resources.Bullet5, Properties.Resources.Bullet6, Properties.Resources.Bullet7
        };
        static float[] radii = new float[] { 4, 2, 3, 13, 3, 3, 2, 2 };
        readonly int imageID = 0;
        readonly int actID = 0;
        readonly bool rotatable = false;  // 速度の方向に向くように回転するか否か
        Vector2 speed = new();

        public Vector2 position = new();
        public bool enable = true;
        public readonly float radius;

        public Bullet(int imageID, int actID, bool rotable, Vector2 initial_position, Vector2 initial_speed)
        {
            this.imageID = imageID;
            this.actID = actID;
            this.rotatable = rotable;
            this.position = initial_position;
            this.speed = initial_speed;
            this.radius = radii[actID];
        }

        public void Progress()
        {
            if (actID == 0)         // 等速直線運動
            {
                position += speed; 
            }
            else if (actID == 1)    // 等加速度直線運動
            {
                const float acceleration = 0.03f;
                float speed_norm = speed.Length();
                speed = (speed_norm + acceleration) * (speed / speed_norm);
                position += speed;
            }
            else if (actID == 2)    // 重力加速度運動
            {
                const float G = 0.15f;
                speed.Y += G;
                position += speed;
            }
            if (position.X < BackGround.position.X || position.X > BackGround.position.X + BackGround.screen_size.Width
                || position.Y < BackGround.position.Y || position.Y > BackGround.position.Y + BackGround.screen_size.Height) enable = false;
        }

        public void Draw(Graphics graphics)
        {
            // 画像の中心がpositionとなるように描画
            var image = images[imageID];    
            if (rotatable)
            {
                var angle = -MathF.Atan2(speed.X, speed.Y);
                float x = image.Width / 2, y = image.Height / 2, c = MathF.Cos(angle), s = MathF.Sin(angle);
                var ps = new Vector2[] { new(-x * c + y * s, -x * s - y * c), new(x * c + y * s, x * s - y * c), new(-x * c - y * s, -x * s + y * c) };
                graphics.DrawImage(image, ps.Select(p => (PointF)(p + position)).ToArray());
            }
            else graphics.DrawImage(image, position.X - image.Width / 2, position.Y - image.Height / 2, image.Width, image.Height);
        }
    }
}
