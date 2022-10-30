using NAudio.Wave;
using System.Windows.Input;

namespace Shooting
{
    internal class Minoriko : ShootingObject
    {
        static Point initial_position = new Point(BackGround.position.X + BackGround.screen_size.Width / 2, BackGround.position.Y + BackGround.screen_size.Height);
        static int initial_life = 3;
        static float high_speed = 4;
        static float low_speed = 2;
        static Image image = Properties.Resources.Minoriko;
        static Rectangle[,] trimRects = new Rectangle[4, 3];
        readonly int width = image.Width / trimRects.GetLength(0);
        readonly int height = image.Height / trimRects.GetLength(1);
        readonly int animInterval = 10;
        int animTime = 0;
        (int x, int y) trimNumber;
        bool comeback = true;
        bool invincible = false;
        int comebackTime = 0;
        bool shootable = false;
        int shootTime = 0;
        List<Shot> shots;
        WaveStream[] soundEffectStreams;
        WaveOut[] soundEffects;

        public PointF position = new(initial_position.X, initial_position.Y);

        public Minoriko(List<Shot> shots, WaveStream[] soundEffectStreams, WaveOut[] soundEffects)
        {
            this.shots = shots;
            this.soundEffectStreams = soundEffectStreams;
            this.soundEffects = soundEffects;
            for (int i = 0; i < trimRects.GetLength(0); i++)
            {
                for (int j = 0; j < trimRects.GetLength(1); j++)
                {
                    trimRects[i, j] = new Rectangle(i * width, j * height, width, height);
                }
            }

            this.shots = shots;
        }

        public void Progress()
        {
            animTime = (animTime + 1) % (animInterval * trimRects.GetLength(0));
            if (comeback)
            {
                shootable = false;
                trimNumber.y = 0;
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

            trimNumber.y = 0;
            float speed = high_speed;
            if (Keyboard.IsKeyDown(Key.LeftShift)) speed = low_speed;   // プロジェクトのプロパティでWPFを有効にすることでKeyboardクラスが使える
            if (Keyboard.IsKeyDown(Key.Right))
            {
                position.X += speed;
                trimNumber.y = 1;
            }
            if (Keyboard.IsKeyDown(Key.Left))
            {
                position.X -= speed;
                trimNumber.y = 2;
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
                        soundEffectStreams[4].Position = 0;
                        soundEffects[4].Play();
                    }
                    shootTime = 0;
                }
            }
        }

        public void Draw(Graphics graphics)
        {
            trimNumber.x = animTime / animInterval;
            if (!invincible || comebackTime % 4 < 2)    // 無敵時間は点滅
                graphics.DrawImage(image, new Rectangle((int)position.X, (int)position.Y, width, height), trimRects[trimNumber.x, trimNumber.y], GraphicsUnit.Pixel);
        }
    }
}
