using NAudio.Wave;
using System.Numerics;
using System.Windows.Input;

namespace Shooting
{
    internal class Minoriko : ShootingObject
    {
        static Vector2 initial_position = new(BackGround.position.X + BackGround.screen_size.Width / 2, BackGround.position.Y + BackGround.screen_size.Height);
        static int initial_life = 3;
        static float high_speed = 4;
        static float low_speed = 2;
        bool comeback = true;
        bool invincible = false;
        int comebackTime = 0;
        bool shootable = false;
        int shootTime = 0;
        List<Shot> shots;
        SoundEffect soundEffect;
        Animation animation = new();

        public Vector2 position = new(initial_position.X, initial_position.Y);

        public Minoriko(List<Shot> shots, SoundEffect soundEffect)
        {
            this.shots = shots;
            this.soundEffect = soundEffect;
        }

        public void Progress()
        {
            animation.Progress();
            if (comeback)
            {
                shootable = false;
                animation.trimNumber.y = 0;
                position.Y -= low_speed;
                if (++comebackTime >= 30)
                {
                    comeback = false;
                    shootable = true;
                }
                return;
            }
            if (invincible)
            {
                if (++comebackTime >= 60)
                {
                    invincible = false;
                    comebackTime = 0;
                }
            }

            animation.trimNumber.y = 0;
            float speed = high_speed;
            if (Keyboard.IsKeyDown(Key.LeftShift)) speed = low_speed;   // プロジェクトのプロパティでWPFを有効にすることでKeyboardクラスが使える
            if (Keyboard.IsKeyDown(Key.Right))
            {
                position.X += speed;
                animation.trimNumber.y = 1;
            }
            if (Keyboard.IsKeyDown(Key.Left))
            {
                position.X -= speed;
                animation.trimNumber.y = 2;
            }
            if (Keyboard.IsKeyDown(Key.Up)) position.Y -= speed;
            if (Keyboard.IsKeyDown(Key.Down)) position.Y += speed;

            if (shootable)
            {
                if (++shootTime >= 3)
                {
                    if (Keyboard.IsKeyDown(Key.Z))
                    {
                        shots.Add(new Shot(new(position.X + 10, position.Y)));
                        shots.Add(new Shot(new(position.X - 10, position.Y)));
                        soundEffect.Play(4);
                    }
                    shootTime = 0;
                }
            }
        }

        public void Draw(Graphics graphics)
        {
            animation.Draw(graphics, position, invincible, comebackTime);
        }

        class Animation
        {
            static Image image = Properties.Resources.Minoriko;
            static Rectangle[,] trimRects = new Rectangle[4, 3];
            readonly int width = image.Width / trimRects.GetLength(0);
            readonly int height = image.Height / trimRects.GetLength(1);
            readonly int interval = 10;
            int time = 0;

            public (int x, int y) trimNumber;

            public Animation()
            {
                for (int i = 0; i < trimRects.GetLength(0); i++) for (int j = 0; j < trimRects.GetLength(1); j++)
                        trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
            }

            public void Progress()
            {
                time = (time + 1) % (interval * trimRects.GetLength(0));
            }

            public void Draw(Graphics graphics, Vector2 position, bool invincible, int comebackTime)
            {
                trimNumber.x = time / interval;
                if (!invincible || comebackTime % 4 < 2)    // 無敵時間は点滅
                    graphics.DrawImage(image, new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height),
                        trimRects[trimNumber.x, trimNumber.y], GraphicsUnit.Pixel);
            }
        }
    }
}
