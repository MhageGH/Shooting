using System.Security.Cryptography.X509Certificates;

namespace AdditiveSynthesisTest
{
    public partial class Form1 : Form
    {
        static Bitmap background = Properties.Resources.ƒLƒƒƒvƒ`ƒƒ;
        static Bitmap effect5 = Properties.Resources.EffectSprite5;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
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
            e.Graphics.DrawImage(background, 0, 0, background.Width, background.Height);
        }
    }
}