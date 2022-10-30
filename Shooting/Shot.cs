namespace Shooting
{
    internal class Shot : ShootingObject
    {
        static Image image = Properties.Resources.Shot;
        static float radius = 10;
        static int effectID = 0;
        public PointF position;
        public bool enable = true;

        public Shot(PointF position)
        {
            this.position = position;
        }

        public void Progress()
        {
            position.Y -= 20;
            if (position.Y < BackGround.position.Y) enable = false;
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(image, position.X, position.Y, image.Width, image.Height);
        }
    }
}
