using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing.Imaging;
using System.Numerics;

namespace Shooting
{
    internal abstract class Effect : ShootingObject
    {
        protected static Bitmap[] images = new Bitmap[]
        {
            Properties.Resources.EffectSprite0, Properties.Resources.EffectSprite1, Properties.Resources.EffectSprite2, Properties.Resources.EffectSprite3,
            Properties.Resources.EffectSprite4, Properties.Resources.EffectSprite5, Properties.Resources.EffectSprite6
        };
        protected static Mat[] mats = images.Select(i => BitmapConverter.ToMat(i)).ToArray();
        protected int time = 0;

        public bool enable = true;

        public abstract void Progress();

        public abstract void Draw(Bitmap canvas);

        /// <summary>
        /// エフェクトを加算合成で描く
        /// </summary>
        static public void DrawEffect(Bitmap canvas, Bitmap image, Vector2 position, int alpha)
        {
            int width = image.Width, height = image.Height;
            var trimmedCanvas = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(trimmedCanvas))
            {
                g.DrawImage(canvas, new Rectangle(0, 0, width, height), new Rectangle((int)position.X, (int)position.Y, width, height), GraphicsUnit.Pixel);
            }
            var canvasBmpData = trimmedCanvas.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var efBmpData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                var bgImgData = new byte[canvasBmpData.Stride * height];
                System.Runtime.InteropServices.Marshal.Copy(canvasBmpData.Scan0, bgImgData, 0, bgImgData.Length);
                var efImgData = new byte[efBmpData.Stride * height];
                System.Runtime.InteropServices.Marshal.Copy(efBmpData.Scan0, efImgData, 0, efImgData.Length);
                for (int i = 0; i < bgImgData.Length; i++) bgImgData[i] = (byte)Math.Min((int)bgImgData[i] + (int)efImgData[i] * alpha / 256, 255);
                System.Runtime.InteropServices.Marshal.Copy(bgImgData, 0, canvasBmpData.Scan0, bgImgData.Length);
            }
            finally
            {
                trimmedCanvas.UnlockBits(canvasBmpData);
                image.UnlockBits(efBmpData);
            }
            using (var g = Graphics.FromImage(canvas))
            {
                g.DrawImage(trimmedCanvas, position.X, position.Y, width, height);
            }
        }
    }

    /// <summary>
    /// 敵ダメージエフェクト
    /// </summary>
    class Effect0 : Effect
    {
        Vector2 position;
        int life_period = 2;

        public Effect0(Vector2 position)
        {
            this.position = position;
        }

        public override void Progress()
        {
            if (time >= life_period) enable = false;
            time++;
        }

        public override void Draw(Bitmap canvas)
        {
            var image = images[0];
            DrawEffect(canvas, image, new(position.X - image.Width / 2, position.Y - image.Height / 2), 255);
        }
    }

    /// <summary>
    /// ピチュンエフェクト
    /// </summary>
    class Effect1 : Effect
    {
        Vector2 position;
        EffectElement0 element0A, element0B;
        EffectElement1[] element1s = new EffectElement1[10];
        int life_period = 5;

        public Effect1(Vector2 position)
        {
            this.position.X = position.X;
            var rand = new Random();
            element0A = new(images[1], position, rand.NextSingle() * MathF.PI, 0.4f);
            element0B = new(images[2], position, 0, 0.6f);
            for (int i = 0; i < element1s.Length; ++i) element1s[i] = new(images[3], position);
        }

        public override void Progress()
        {
            element0A.Progress();
            element0B.Progress();
            foreach (var element1 in element1s) element1.Progress();
            if (time >= life_period) enable = false;
            ++time;
        }

        public override void Draw(Bitmap canvas)
        {
            element0A.Draw(canvas);
            element0B.Draw(canvas);
            foreach (var element1 in element1s) element1.Draw(canvas);
        }


    }

    /// <summary>
    /// ボス用魔方陣エフェクト。持続型。終了時はenableをfalseにする。
    /// </summary>
    class Effect2 : Effect
    {
        float angle = 0;
        float scale = 0.5f;
        bool in_scale_up = true;
        const float angular_speed = 0.1f;
        const float scale_speed = 0.01f;
        const float scale_max = 1;
        const float scale_min = 0.8f;
        Shizuha shizuha;

        public Effect2(Shizuha shizuha)
        {
            this.shizuha = shizuha;
        }

        public override void Progress()
        {
            angle += angular_speed;
            if (in_scale_up)
            {
                scale += scale_speed;
                if (scale > scale_max) in_scale_up = false;
            }
            else
            {
                scale -= scale_speed;
                if (scale < scale_min) in_scale_up = true;
            }
        }

        public override void Draw(Bitmap canvas)
        {
            var image = images[4];
            var graphics = Graphics.FromImage(canvas);
            float x = image.Width / 2, y = image.Height / 2, c = MathF.Cos(angle), s = MathF.Sin(angle);
            var points = new Vector2[] { new(-x * c + y * s, -x * s - y * c), new(x * c + y * s, x * s - y * c), new(-x * c - y * s, -x * s + y * c) };
            graphics.DrawImage(image, points.Select(point => (PointF)(point * scale + shizuha.position)).ToArray());
        }
    }

    /// <summary>
    /// 敵弾発射エフェクト
    /// </summary>
    class Effect3 : Effect
    {
        Vector2 position;
        const int life_period = 3;
        float angle;

        public Effect3(Vector2 position)
        {
            this.position = position;
        }

        public override void Progress()
        {
            if (time == life_period) enable = false;
            ++time;
        }

        public override void Draw(Bitmap canvas)
        {
            var random = new Random();
            angle = random.NextSingle() * 2 * MathF.PI;
            var image = images[5];
            var r = MathF.Sqrt(MathF.Pow(image.Width / 2.0f, 2) + MathF.Pow(image.Height / 2.0f, 2));
            var img = new Bitmap((int)(2 * r), (int)(2 * r));
            var graphics = Graphics.FromImage(img);
            graphics.Clear(Color.Transparent);
            float x = image.Width / 2, y = image.Height / 2, c = MathF.Cos(angle), s = MathF.Sin(angle);
            var vecs = new Vector2[] { new(-x * c + y * s, -x * s - y * c), new(x * c + y * s, x * s - y * c), new(-x * c - y * s, -x * s + y * c) };
            graphics.DrawImage(image, vecs.Select(v => (PointF)(v + new Vector2(img.Width / 2, img.Height / 2))).ToArray());
            Effect.DrawEffect(canvas, img, new(position.X - img.Width / 2, position.Y - img.Height / 2), 200);
        }
    }

    /// <summary>
    /// キュイーン！エフェクト
    /// </summary>
    class Effect4 : Effect
    {
        Vector2 position;
        EffectElement2[] element2s = new EffectElement2[10];
        const int life_period = 200;

        public Effect4(Vector2 position)
        {
            this.position = position;
            for (int i = 0; i < element2s.Length; ++i) element2s[i] = new(images[6], position);
        }


        public override void Progress()
        {
            foreach (var element2 in element2s) element2.Progress();
            if (time == life_period) enable = false;
            ++time;
        }

        public override void Draw(Bitmap canvas)
        {
            foreach (var element2 in element2s) element2.Draw(canvas);
        }
    }

    /// <summary>
    /// 広がって消えるエフェクト要素
    /// </summary>
    class EffectElement0
    {
        float scale = 0;
        int alpha = 100;
        Vector2 position;
        Bitmap image;
        float angle;
        float scaleUpSpeed;
        const int alpha_down_speed = 5;

        public EffectElement0(Bitmap image, Vector2 position, float angle, float scaleUpSpeed)
        {
            this.position = position;
            this.image = image;
            this.angle = angle;
            this.scaleUpSpeed = scaleUpSpeed;
        }

        public void Progress()
        {
            scale += scaleUpSpeed;
            alpha -= alpha_down_speed;
            if (alpha < 0) alpha = 0;
        }

        public void Draw(Bitmap canvas)
        {
            if (scale == 0) return;
            var r = MathF.Sqrt(MathF.Pow(image.Width / 2.0f, 2) + MathF.Pow(image.Height / 2.0f, 2));
            var img = new Bitmap((int)(2 * r * scale), (int)(2 * r * scale));
            var graphics = Graphics.FromImage(img);
            graphics.Clear(Color.Transparent);
            float x = image.Width / 2, y = image.Height / 2, c = MathF.Cos(angle), s = MathF.Sin(angle);
            var vecs = new Vector2[] { new(-x * c + y * s, -x * s - y * c), new(x * c + y * s, x * s - y * c), new(-x * c - y * s, -x * s + y * c) };
            graphics.DrawImage(image, vecs.Select(v => (PointF)(v * scale + new Vector2(img.Width / 2, img.Height / 2))).ToArray());
            Effect.DrawEffect(canvas, img, new(position.X - img.Width / 2, position.Y - img.Height / 2), alpha);
        }
    }

    /// <summary>
    /// 飛び散って消えるエフェクト要素
    /// </summary>
    class EffectElement1
    {
        Bitmap image;
        Vector2 position;
        const float max_away_speed = 20;
        const float max_scale = 2;
        const float scale_down_speed = 0.06f;
        const int alpha_down_speed = 10;
        int alpha = 127;
        float away_speed;
        float angle;
        float scale;

        public EffectElement1(Bitmap image, Vector2 position)
        {
            this.image = image;
            this.position = position;
            var random = new Random();
            away_speed = (random.NextSingle() * 0.5f + 0.5f) * max_away_speed;
            angle = random.NextSingle() * 2.0f * MathF.PI;
            scale = (random.NextSingle() * 0.5f + 0.5f) * max_scale;
        }

        public void Progress()
        {
            scale -= scale_down_speed;
            if (scale < 0) scale = 0;
            alpha -= alpha_down_speed;
            if (alpha < 0) alpha = 0;
            position.X += away_speed * MathF.Cos(angle);
            position.Y += away_speed * MathF.Sin(angle);
        }

        public void Draw(Bitmap canvas)
        {
            if (scale == 0) return;
            Effect.DrawEffect(canvas, image, new(position.X - image.Width / 2, position.Y - image.Height / 2), alpha);
        }
    }

    /// <summary>
    /// 吸い込まれるエフェクト要素
    /// </summary>
    class EffectElement2
    {
        Bitmap image;
        Vector2 position;
        Vector2 convergence_point;
        bool visible = true;
        const float r_min = 50;
        const float r_max = 500;
        float r, angle;
        const float speed = 10;

        public EffectElement2(Bitmap image, Vector2 convergence_point)
        {
            this.image = image;
            this.convergence_point = convergence_point;
            var random = new Random();
            r = r_min + random.NextSingle() * (r_max - r_min);
            var alpha = random.NextSingle() * 2 * MathF.PI;
            position.X = r * MathF.Cos(alpha) + convergence_point.X;
            position.Y = r * MathF.Sin(alpha) + convergence_point.Y;
            angle = random.NextSingle() * 2 * MathF.PI;
        }

        public void Progress()
        {
            var v = convergence_point - position;
            var l = v.Length();
            if (l < speed) visible = false;
            else position += speed * v / l;
        }

        public void Draw(Bitmap canvas)
        {
            if (!visible) return;
            var r = MathF.Sqrt(MathF.Pow(image.Width / 2.0f, 2) + MathF.Pow(image.Height / 2.0f, 2));
            var img = new Bitmap((int)(2 * r), (int)(2 * r));
            var graphics = Graphics.FromImage(img);
            graphics.Clear(Color.Transparent);
            float x = image.Width / 2, y = image.Height / 2, c = MathF.Cos(angle), s = MathF.Sin(angle);
            var vecs = new Vector2[] { new(-x * c + y * s, -x * s - y * c), new(x * c + y * s, x * s - y * c), new(-x * c - y * s, -x * s + y * c) };
            graphics.DrawImage(image, vecs.Select(v => (PointF)(v + new Vector2(img.Width / 2, img.Height / 2))).ToArray());
            Effect.DrawEffect(canvas, img, new(position.X - img.Width / 2, position.Y - img.Height / 2), 255);
        }
    }
}
