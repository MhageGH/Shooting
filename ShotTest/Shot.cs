using System.Numerics;

namespace ShotTest
{
    internal class Shot
    {
        static Image image = Resource.Shot;
        public Vector2 position;
        public bool enable = true;

        public Shot(Vector2 position)
        {
            this.position = position;
        }

        public void Progress()
        {
            position.Y -= 20;
            if (position.Y < 0) enable = false;
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(image, position.X - image.Width / 2, position.Y - image.Height / 2, image.Width, image.Height);
        }
    }
}
