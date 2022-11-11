using System.Drawing.Imaging;

namespace AdditiveSynthesisTest
{
    public partial class Form1 : Form
    {
        static Bitmap background = To32bppArgb(Properties.Resources.ƒLƒƒƒvƒ`ƒƒ);
        static Bitmap effect5 = To32bppArgb(Properties.Resources.EffectSprite5);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Paint1(e.Graphics);
            Paint2(e.Graphics);
        }

        static Bitmap To32bppArgb(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == PixelFormat.Format32bppArgb) return bitmap;
            var bitmap32 = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap32))
            {
                graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height); 
            }
            return bitmap32;
        }


        void Paint1(Graphics graphics)
        {
            for (int i = 0; i < effect5.Width; i++)
            {
                for (int j = 0; j < effect5.Height; ++j)
                {
                    for (int k = 0; k < 10; ++k)
                    {
                        int x = 150 + 10 * k;
                        int y = 50 + 10 * k;
                        var R = background.GetPixel(x + i, y + j).R + effect5.GetPixel(i, j).R;
                        var G = background.GetPixel(x + i, y + j).G + effect5.GetPixel(i, j).G;
                        var B = background.GetPixel(x + i, y + j).B + effect5.GetPixel(i, j).B;
                        background.SetPixel(x + i, y + j, Color.FromArgb(255, Math.Min(R, 255), Math.Min(G, 255), Math.Min(B, 255)));
                    }
                }
            }
            graphics.DrawImage(background, 0, 0, background.Width, background.Height);
        }

        void Paint2(Graphics graphics)
        {
            var canvas = new Bitmap(background.Width, background.Height);
            using (var g = Graphics.FromImage(canvas))
            {
                g.DrawImage(background, 0, 0, background.Width, background.Height);
            }
            for (int i = 0; i < 10; ++i) DrawEffect(canvas, new(150 + 10 * i, 50 + 10 * i));
            graphics.DrawImage(canvas, 0, 0, canvas.Width, canvas.Height);
        }

        void DrawEffect(Bitmap canvas, Point position)
        {
            int width = effect5.Width, height = effect5.Height;
            var trimmedCanvas = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(trimmedCanvas))
            {
                g.DrawImage(canvas, new Rectangle(0, 0, width, height), new Rectangle(position.X, position.Y, width, height), GraphicsUnit.Pixel);
            }
            var canvasBmpData = trimmedCanvas.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var efBmpData = effect5.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                var bgImgData = new byte[canvasBmpData.Stride * height];
                System.Runtime.InteropServices.Marshal.Copy(canvasBmpData.Scan0, bgImgData, 0, bgImgData.Length);
                var efImgData = new byte[efBmpData.Stride * height];
                System.Runtime.InteropServices.Marshal.Copy(efBmpData.Scan0, efImgData, 0, efImgData.Length);
                for (int i = 0; i < bgImgData.Length; i++) bgImgData[i] = (byte)Math.Min((int)bgImgData[i] + (int)efImgData[i], 255);
                System.Runtime.InteropServices.Marshal.Copy(bgImgData, 0, canvasBmpData.Scan0, bgImgData.Length);
            }
            finally
            {
                trimmedCanvas.UnlockBits(canvasBmpData);
                effect5.UnlockBits(efBmpData);
            }
            using (var g = Graphics.FromImage(canvas))
            {
                g.DrawImage(trimmedCanvas, position.X, position.Y, width, height);
            }
        }
    }
}