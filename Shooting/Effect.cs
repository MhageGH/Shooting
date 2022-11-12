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
        protected Vector2 position;
        protected int time = 0;

        public bool enable = true;

        public Effect(Vector2 position)
        {
            this.position = position;
        }

        public abstract void Progress();

        public abstract void Draw(Bitmap canvas);

        static protected void DrawEffect(Bitmap canvas, Bitmap image, Vector2 position, int alpha)
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
        public Effect0(Vector2 position) : base(position) { }

        public override void Progress()
        {
            if (time >= 2) enable = false;
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
        float scale = 0;
        int alpha = 100;
        Element1 magic_circle, ring;
        Element2[] bits = new Element2[10];

        public Effect1(Vector2 position) : base(position)
        {
            var rand = new Random();
            magic_circle = new(images[1], position, rand.NextSingle() * MathF.PI, 0.4f);
            ring = new(images[2], position, 0, 0.6f);
            for (int i = 0; i < bits.Length; ++i) bits[i] = new(images[3], position);
        }

        public override void Progress()
        {
            magic_circle.Progress();
            ring.Progress();
            foreach (var bit in bits) bit.Progress();
            if (time >= 5) enable = false;
            ++time;
        }

        public override void Draw(Bitmap canvas)
        {
            magic_circle.Draw(canvas);
            ring.Draw(canvas);
            foreach (var bit in bits) bit.Draw(canvas);
        }

        /// <summary>
        /// 時間と共に膨張し、消えていくエフェクト
        /// </summary>
        class Element1
        {
            float scale = 0;
            int alpha = 100;
            Vector2 position;
            Bitmap image;
            float angle;
            float scaleUpSpeed;

            public Element1(Bitmap image, Vector2 position, float angle, float scaleUpSpeed)
            {
                this.position = position;
                this.image = image;
                this.angle = angle;
                this.scaleUpSpeed = scaleUpSpeed;
            }

            public void Progress()
            {
                scale += scaleUpSpeed;
                alpha -= 5;
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
                DrawEffect(canvas, img, new(position.X - img.Width / 2, position.Y - img.Height / 2), alpha);
            }
        }

        /// <summary>
        /// 発生位置からランダムな方向に時間と共に遠ざかり、小さくなっていくエフェクト
        /// </summary>
        class Element2
        {
            Bitmap image;
            Vector2 position;

            public Element2(Bitmap image, Vector2 position)
            {
                this.image = image;
                this.position = position;
            }

            public void Progress()
            {

            }

            public void Draw(Bitmap canvas)
            {

            }
        }
    }

    class Effect2 : Effect
    {
        public Effect2(Vector2 position) : base(position) { }

        public override void Progress()
        {

        }

        public override void Draw(Bitmap canvas)
        {

        }
    }

    class Effect3 : Effect
    {
        public Effect3(Vector2 position) : base(position) { }

        public override void Progress()
        {

        }

        public override void Draw(Bitmap canvas)
        {

        }
    }
    class Effect4 : Effect
    {
        public Effect4(Vector2 position) : base(position) { }

        public override void Progress()
        {

        }

        public override void Draw(Bitmap canvas)
        {

        }
    }
}
