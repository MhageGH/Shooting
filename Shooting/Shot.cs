using System.Numerics;

namespace Shooting
{
    internal class Shot : ShootingObject
    {
        static Image image = Properties.Resources.Shot;
        static int speed = 20;
        static float radius = 10;
        static int effectID = 0;
        public Vector2 position;
        public bool enable = true;

        public Shot(Vector2 position)
        {
            this.position = position;
        }

        public void Progress()
        {
            position.Y -= speed;
            if (position.Y < BackGround.position.Y) enable = false;
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(image, position.X, position.Y, image.Width, image.Height);
        }
    }
}
