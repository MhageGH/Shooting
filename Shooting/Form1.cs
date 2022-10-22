
namespace Shooting
{
    public partial class Form1 : Form
    {
        int animationTime = 0;
        BackGround backGround = new BackGround();
        Image imageMinoriko = Properties.Resources.Minoriko;
        Image imageBullet1 = Properties.Resources.Bullet1;
        Image imageBullet2 = Properties.Resources.Bullet2;
        List<Bullet> bullet1s = new List<Bullet>();
        List<Bullet> bullet2s = new List<Bullet>();
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Invalidate();
            animationTime = (animationTime + 1) % 40;
            if (animationTime % 5 == 0)
            {
                bullet1s.Add(new Bullet(new Point(100, 0), new Point(2, 10), imageBullet1));
                bullet1s.Add(new Bullet(new Point(150, 0), new Point(1, 10), imageBullet1));
                bullet1s.Add(new Bullet(new Point(200, 0), new Point(0, 10), imageBullet1));
                bullet1s.Add(new Bullet(new Point(250, 0), new Point(-1, 10), imageBullet1));
                bullet1s.Add(new Bullet(new Point(300, 0), new Point(-2, 10), imageBullet1));
                bullet2s.Add(new Bullet(new Point(210, 0), new Point(4, 5), imageBullet2));
                bullet2s.Add(new Bullet(new Point(265, 0), new Point(2, 5), imageBullet2));
                bullet2s.Add(new Bullet(new Point(310, 0), new Point(0, 5), imageBullet2));
                bullet2s.Add(new Bullet(new Point(360, 0), new Point(-2, 5), imageBullet2));
                bullet2s.Add(new Bullet(new Point(410, 0), new Point(-4, 5), imageBullet2));
            }
            for (int i = 0; i < bullet1s.Count; i++)
            {
                bullet1s[i].position.X += bullet1s[i].speed.X;
                bullet1s[i].position.Y += bullet1s[i].speed.Y;
            }
            for (int i = 0; i < bullet2s.Count; i++)
            {
                bullet2s[i].position.X += bullet2s[i].speed.X;
                bullet2s[i].position.Y += bullet2s[i].speed.Y;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            backGround.Draw(e.Graphics);
            int n = animationTime / 10;
            e.Graphics.DrawImage(imageMinoriko, 220, 400, new Rectangle(50 * n, 0, 50, 50), GraphicsUnit.Pixel);

            for (int i = 0; i < bullet1s.Count; i++)
            {
                //e.Graphics.DrawImage(bullet1s[i].image, bullet1s[i].position);
            }
            for (int i = 0; i < bullet2s.Count; i++)
            {
                //e.Graphics.DrawImage(bullet2s[i].image, bullet2s[i].position);
            }
        }
    }

    class Bullet
    {
        public Point position;
        public Point speed;
        public Image image;
        public Bullet(Point position, Point speed, Image image)
        {
            this.position = position;
            this.speed = speed;
            this.image = image;
        }
    }
}