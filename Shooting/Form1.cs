using NAudio.Wave;

namespace Shooting
{
    public partial class Form1 : Form
    {
        BackGround backGround = new ();
        Minoriko minoriko = new ();
        Image imageFrame = Properties.Resources.Frame;
        WaveStream bgmStream = new Mp3FileReader(new System.IO.MemoryStream(Properties.Resources.BGM0));
        static WaveStream[] soundEffectStreams = new WaveFileReader[] {
            new (Properties.Resources.SE0), new(Properties.Resources.SE1), new(Properties.Resources.SE2),new(Properties.Resources.SE3),
            new (Properties.Resources.SE4), new(Properties.Resources.SE5), new(Properties.Resources.SE6),new(Properties.Resources.SE7),
            new (Properties.Resources.SE8), new(Properties.Resources.SE9), new(Properties.Resources.SE10),new(Properties.Resources.SE11)
        };
        WaveOut bgm = new();
        WaveOut[] soundEffects = new WaveOut[soundEffectStreams.Length].Select(s => s = new()).ToArray();

        public Form1()
        {
            InitializeComponent();
            backGround.SpellEnable = false;
            bgm.Init(bgmStream);
            bgm.Volume = 0.5f; // NAudioのボリュームはstaticクラスに結びついているため全WaveOutオブジェクトに適用される。個別の音量調整は出来ない。予め比率を調整しておく。
            bgm.Play();
            for (int i = 0; i < soundEffects.Length; ++i) soundEffects[i].Init(soundEffectStreams[i]);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bgmStream.Position == bgmStream.Length)
            {
                bgmStream.Position = 0;
                bgm.Play();
            }
            Invalidate();
            backGround.Progress();
            minoriko.Progress();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // 注意: DrawImage(Image, Point)は元の物理サイズが適用されるのでNG。WidthとHeightを指定すること。
            backGround.Draw(e.Graphics);
            minoriko.Draw(e.Graphics);
            e.Graphics.DrawImage(imageFrame, 0, 0, imageFrame.Width, imageFrame.Height);
        }
    }
}