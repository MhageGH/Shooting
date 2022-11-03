using Microsoft.VisualBasic;
using NAudio.Wave;
using System.Numerics;

namespace Shooting
{
    internal class Shizuha : ShootingObject
    {
        int state = 0;
        Minoriko minoriko;
        static Vector2 initial_position = new(BackGround.position.X + BackGround.screen_size.Width / 2, 100);
        public Vector2 position = initial_position + new Vector2(200, 200);
        public bool spellEnable;
        Animation animation = new();
        Mover mover = new();
        Attacker attacker;
        SoundEffect soundEffect;


        public Shizuha(Minoriko minoriko, SoundEffect soundEffect, List<Bullet> bullets)
        {
            this.minoriko = minoriko;
            this.soundEffect = soundEffect;
            attacker = new Attacker(bullets);
        }

        public void Progress()
        {
            animation.Progress();
            switch (state)
            {
                case 0:
                    if (mover.time == 0) soundEffect.Play(11);    // 溜め効果音
                    if (mover.Move0(ref position) == true)
                    {
                        mover.time = 0;
                        state = 1;
                    }
                    break;
                case 1:
                    mover.Move1(ref position, minoriko.position);
                    attacker.Attack0(position, minoriko.position);
                    break;
            }
        }

        public void Draw(Graphics graphics)
        {
            animation.Draw(graphics, position, mover.speed);
        }

        class Animation
        {
            static Image image = Properties.Resources.Shizuha;
            static Rectangle[,] trimRects = new Rectangle[4, 3];
            readonly int width = image.Width / trimRects.GetLength(0);
            readonly int height = image.Height / trimRects.GetLength(1);
            readonly int interval = 10;
            int time = 0;
            (int x, int y) trimNumber;

            public Animation()
            {
                for (int i = 0; i < trimRects.GetLength(0); i++) for (int j = 0; j < trimRects.GetLength(1); j++)
                        trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
            }

            public void Progress()
            {
                time = (time + 1) % (interval * trimRects.GetLength(0));
            }

            public void Draw(Graphics graphics, Vector2 position, Vector2 speed)
            {
                trimNumber.x = time / interval;
                trimNumber.y = 0;
                if (speed.X > 0) trimNumber.y = 1;
                if (speed.X < 0) trimNumber.y = 2;
                graphics.DrawImage(image, new Rectangle((int)position.X, (int)position.Y, width, height), trimRects[trimNumber.x, trimNumber.y], GraphicsUnit.Pixel);
            }
        }

        class Mover
        {
            public int time = 0;
            int endOfTime = 0;
            const float speed_norm = 4;
            public Vector2 speed = new(0, 0);

            /// <summary>
            /// 一定速度で初期位置に戻り、一定時間経過後trueを返す
            /// </summary>
            public bool Move0(ref Vector2 position)
            {
                const int waitTime = 60;
                var v = initial_position - position;
                speed = v / v.Length() * speed_norm;
                if (v.Length() > 2 * speed_norm) position += speed;
                else speed = new(0, 0);
                if (++time >= waitTime && speed.LengthSquared() < 0.1) return true;
                return false;
            }

            /// <summary>
            /// 一定間隔ごとに一定速度で、ランダムな時間、移動可能範囲内を移動する。
            /// 方向は穣子の方を向くx座標の単位ベクトルを-45～+45°のランダム角で回転させた方向
            /// </summary>
            public void Move1(ref Vector2 position, Vector2 minoriko_position)
            {
                const int interval = 45;
                const int endOfTime_min = 10;
                const int endOfTime_max = 25;
                int x_min = (int)BackGround.position.X + 100;
                int x_max = (int)BackGround.position.X + BackGround.screen_size.Width - 100;
                int y_min = 80;
                int y_max = 150;
                if (time == 0)
                {
                    var direction = new Vector2();
                    var rand = new Random();
                    endOfTime = (int)((endOfTime_max - endOfTime_min) * rand.NextSingle() + endOfTime_min);
                    var theta = (Math.PI / 2) * (rand.NextDouble() - 0.5);
                    var cos = (float)Math.Cos(theta);
                    var sin = (float)Math.Sin(theta);
                    var asin = (float)Math.Abs(sin);
                    direction.X = ((minoriko_position.X - position.X >= 0 && position.X < x_max) || position.X < x_min) ? cos : -cos;
                    direction.Y = (position.Y < y_min) ? asin : (position.Y > y_max) ? -asin : sin;
                    speed = speed_norm * direction;
                }
                else if (time < endOfTime) position += speed;
                else speed = new(0, 0);
                if (++time >= interval) time = 0;
            }
        }

        class Attacker
        {
            const int start_time = 120;
            int time = 0;
            BulletMaker bulletMaker;

            public Attacker(List<Bullet> bullets)
            {
                bulletMaker = new BulletMaker(0, bullets);
            }

            public void Attack0(Vector2 source_position, Vector2 target_position)
            {
                bulletMaker.Make(source_position, target_position);
            }

            public void Attack1()
            {
                // TODO
            }

            public void Attack2()
            {
                // TODO
            }

            public void Attack3()
            {
                // TODO
            }
        }
    }
}