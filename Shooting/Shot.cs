namespace Shooting
{
    internal class Shot : ShootingObject
    {
        static Image image = Properties.Resources.Shot;
        PointF position;

        public Shot(PointF position)
        {
            this.position = position;
        }

        public void Progress()
        {

        }

        public void Draw(Graphics graphics)
        {

        }
    }
}
