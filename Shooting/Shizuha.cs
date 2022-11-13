﻿using Microsoft.VisualBasic;
using NAudio.Wave;
using System.Drawing.Imaging;
using System.Numerics;
using System.Windows.Media.Animation;

namespace Shooting
{
    internal class Shizuha : ShootingObject
    {
        int state = 0;
        Minoriko minoriko;
        static Vector2 initial_position = new(BackGround.position.X + BackGround.screen_size.Width / 2, 100);
        Animation animation = new();
        Mover mover = new();
        Attacker attacker;
        SoundEffect soundEffect;
        List<Bullet> bullets;
        List<Effect> effects;
        Effect? effect2 = null;
        CutIn cutIn = new();

        public float radius = 20;
        public Vector2 position = initial_position;
        public bool spellCard = false;
        public int power_max = 50;
        public int power = 50;
        public bool gameClear = false;
        public bool enable = true;

        public Shizuha(Minoriko minoriko, SoundEffect soundEffect, List<Bullet> bullets, List<Effect> effects)
        {
            this.minoriko = minoriko;
            this.soundEffect = soundEffect;
            this.bullets = bullets;
            this.effects = effects;
            attacker = new Attacker(bullets, effects);
        }

        public void Progress()
        {
            animation.Progress();
            if (cutIn.enable) cutIn.Progress();
            switch (state)
            {
                case 0:
                    if (mover.time == 0)
                    {
                        soundEffect.Play(11);    // 溜め効果音
                        effects.Add(new Effect4(initial_position));
                    }
                    if (mover.Move0(ref position) == true)
                    {
                        mover.time = 0;
                        attacker.time = 0;
                        power_max = power = 50;
                        state = 1;
                        effect2 = new Effect2(this);
                        effects.Add(effect2);
                    }
                    break;
                case 1:
                    mover.Move1(ref position, minoriko.position);
                    attacker.Attack0(position, minoriko.position);
                    if (power <= 0)
                    {
                        mover.time = 0;
                        attacker.time = 0;
                        power_max = power = 50;
                        if (effect2 != null) effect2.enable = false;
                        state = 2;
                    }
                    break;
                case 2:
                    if (mover.time == 0)
                    {
                        cutIn.Start();
                        soundEffect.Play(7);
                        soundEffect.Play(11);
                        effects.Add(new Effect4(initial_position));
                    }
                    if (mover.Move0(ref position) == true)
                    {
                        mover.time = 0;
                        attacker.time = 0;
                        effect2 = new Effect2(this);
                        effects.Add(effect2);
                        state = 3;
                    }
                    break;
                case 3:
                    mover.Move1(ref position, minoriko.position);
                    attacker.Attack1(position, minoriko.position);
                    if (power <= 0)
                    {
                        mover.time = 0;
                        attacker.time = 0;
                        power_max = power = 50;
                        if (effect2 != null) effect2.enable = false;
                        state = 4;
                    }
                    break;
                case 4:
                    if (mover.time == 0)
                    {
                        soundEffect.Play(11);
                        effects.Add(new Effect4(initial_position));
                    }
                    if (mover.Move0(ref position) == true)
                    {
                        mover.time = 0;
                        attacker.time = 0;
                        effect2 = new Effect2(this);
                        effects.Add(effect2);
                        state = 5;
                    }
                    break;
                case 5:
                    mover.Move1(ref position, minoriko.position);
                    attacker.Attack2(position, minoriko.position);
                    if (power <= 0)
                    {
                        mover.time = 0;
                        attacker.time = 0;
                        power_max = power = 50;
                        if (effect2 != null) effect2.enable = false;
                        state = 6;
                    }
                    break;
                case 6:
                    if (mover.time == 0)
                    {
                        cutIn.Start();
                        soundEffect.Play(7);
                        soundEffect.Play(11);
                        effects.Add(new Effect4(initial_position));
                    }
                    if (mover.Move0(ref position) == true)
                    {
                        mover.time = 0;
                        attacker.time = 0;
                        effect2 = new Effect2(this);
                        effects.Add(effect2);
                        state = 7;
                    }
                    break;
                case 7:
                    mover.Move1(ref position, minoriko.position);
                    attacker.Attack3(position, minoriko.position);
                    if (power <= 0)
                    {
                        mover.time = 0;
                        attacker.time = 0;
                        if (effect2 != null) effect2.enable = false;
                        state = 8;
                    }
                    break;
                case 8:
                    if (enable) soundEffect.Play(6);
                    gameClear = true;
                    foreach (var bullet in bullets) bullet.enable = false;
                    enable = false;
                    break;
            }
        }

        public void Draw(Bitmap canvas)
        {
            var graphics = Graphics.FromImage(canvas);
            animation.Draw(graphics, position, mover.speed);
            if (cutIn.enable) cutIn.Draw(graphics);
        }

        public void ReceiveDamage()
        {
            power--;
            soundEffect.Play(5);
            effects.Add(new Effect0(position));
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
                graphics.DrawImage(image, new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height), 
                    trimRects[trimNumber.x, trimNumber.y], GraphicsUnit.Pixel);
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
                int y_min = 100;
                int y_max = 150;
                if (time == 0)
                {
                    var direction = new Vector2();
                    var rand = new Random();
                    endOfTime = (int)((endOfTime_max - endOfTime_min) * rand.NextSingle() + endOfTime_min);
                    var theta = (MathF.PI / 2) * (rand.NextSingle() - 0.5f);
                    var cos = MathF.Cos(theta);
                    var sin = MathF.Sin(theta);
                    var asin = MathF.Abs(sin);
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
            public int time = 0;
            BulletMaker bulletMaker;

            public Attacker(List<Bullet> bullets, List<Effect> effects)
            {
                bulletMaker = new BulletMaker(bullets, effects);
            }

            public void Attack0(Vector2 source_position, Vector2 target_position)
            {
                if (time == 0) bulletMaker.time = 0;
                if (50 < time && time % 150 < 100)
                {
                    bulletMaker.Make(source_position, target_position, 0);
                    bulletMaker.Make(source_position, target_position, 1);
                    bulletMaker.Make(source_position, target_position, 2);
                }
                time++;
                bulletMaker.time++;
            }

            public void Attack1(Vector2 source_position, Vector2 target_position)
            {
                if (time == 0) bulletMaker.time = 0;
                if (time >= 50)
                {
                    bulletMaker.Make(source_position, target_position, 3);
                    bulletMaker.Make(source_position, target_position, 4);
                    bulletMaker.Make(source_position, target_position, 5);
                    bulletMaker.Make(source_position, target_position, 6);
                    bulletMaker.Make(source_position, target_position, 7);
                }
                time++;
                bulletMaker.time++;
            }

            public void Attack2(Vector2 source_position, Vector2 target_position)
            {
                if (time == 0) bulletMaker.time = 0;
                if (time >= 50)
                {
                    bulletMaker.Make(source_position, target_position, 8);
                    bulletMaker.Make(source_position, target_position, 9);
                }
                time++;
                bulletMaker.time++;
            }

            public void Attack3(Vector2 source_position, Vector2 target_position)
            {
                if (time == 0) bulletMaker.time = 0;
                if (time >= 50)
                {
                    bulletMaker.Make(source_position, target_position, 10);
                    bulletMaker.Make(source_position, target_position, 11);
                }
                time++;
                bulletMaker.time++;
            }
        }

        class CutIn
        {
            static Image image = Properties.Resources.BigShizuha;
            public bool enable = false;
            int alpha = 0;
            Vector2 position;
            readonly Vector2 initial_position = new(422, 420);
            const float left_end = 228;
            const float speed_to_left = 20;
            const float speed_to_up = 10;
            const int alpha_up_speed = 24;
            const int alpha_down_speed = 4;

            public void Start()
            {
                enable = true;
                position = initial_position;
                alpha = 0;
            }

            public void Progress()
            {
                if (position.X > left_end)
                {
                    position.X -= speed_to_left;
                    alpha += alpha_up_speed;
                    if (alpha > 255) alpha = 255;
                }
                else
                {
                    position.Y -= speed_to_up;
                    alpha -= alpha_down_speed;
                    if (alpha <= 0)
                    {
                        alpha = 0;
                        enable = false;
                    }
                }
            }

            public void Draw(Graphics graphics)
            {
                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(new() { Matrix00 = 1, Matrix11 = 1, Matrix22 = 1, Matrix33 = (float)alpha / 255, Matrix44 = 1 });
                graphics.DrawImage(image, new Rectangle((int)position.X - image.Width / 2, (int)position.Y - image.Height / 2, image.Width, image.Height),
                    0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
            }

        }
    }
}