using OpenCvSharp;
using OpenCvSharp.Extensions;
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

        protected void AdditiveSynthesis(Bitmap canvas, Bitmap image, int x, int y)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; ++j)
                {
                    var R = Math.Min(canvas.GetPixel(x + i, y + j).R + image.GetPixel(i, j).R, 255);
                    var G = Math.Min(canvas.GetPixel(x + i, y + j).G + image.GetPixel(i, j).G, 255);
                    var B = Math.Min(canvas.GetPixel(x + i, y + j).B + image.GetPixel(i, j).B, 255);
                    canvas.SetPixel(x + i, y + j, Color.FromArgb(255, R, G, B));
                }
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
            AdditiveSynthesis(canvas, image, (int)position.X - image.Width / 2, (int)position.Y - image.Height / 2);
        }
    }

    class Effect1 : Effect
    {
        public Effect1(Vector2 position) : base(position) { }

        public override void Progress()
        {

        }

        public override void Draw(Bitmap canvas)
        {

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
