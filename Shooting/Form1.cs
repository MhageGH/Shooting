using NAudio.Wave;
using System.Linq;

namespace Shooting
{
    public partial class Form1 : Form
    {
        static Image imageFrame = Properties.Resources.Frame;
        static WaveStream bgmStream = new Mp3FileReader(new System.IO.MemoryStream(Properties.Resources.BGM0));
        static WaveOut bgm = new();
        static SoundEffect soundEffect = new();
        static List<Shot> shots = new ();
        static List<Bullet> bullets = new();
        static BackGround backGround = new();
        static Minoriko minoriko = new(shots, soundEffect);
        static Shizuha shizuha = new(minoriko, soundEffect, bullets);
        static ShootingObject[] shootingObjects = new ShootingObject[] { backGround, minoriko, shizuha };

        public Form1()
        {
            InitializeComponent();
            backGround.SpellEnable = false;
            bgm.Init(bgmStream);
            bgm.Volume = 0.5f; // NAudioのボリュームはstaticクラスに結びついているため全WaveOutオブジェクトに適用される。個別の音量調整は出来ない。音源の方で予め比率を調整しておく。
            bgm.Play();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bgmStream.Position == bgmStream.Length)
            {
                bgmStream.Position = 0;
                bgm.Play();
            }
            Invalidate();
            foreach (var shootingObject in shootingObjects) shootingObject.Progress();
            foreach (var shot in shots) shot.Progress();
            shots.RemoveAll(s => s.enable == false);
            foreach(var bullet in bullets)
            {
                bullet.Progress();
                var p = bullet.position - minoriko.position;
                if (p.Length() < bullet.radius + minoriko.radius && !minoriko.invincible)
                {
                    foreach (var b in bullets) b.enable = false;
                    minoriko.Die();
                    if (minoriko.life == 0)
                    {
                        timer1.Stop();
                        MessageBox.Show("Gameover");
                    }
                    break;
                }
            }
            bullets.RemoveAll(b => b.enable == false);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // 注意: DrawImage(Image, Point)は元の物理サイズが適用されるのでNG。WidthとHeightを指定すること。
            backGround.Draw(e.Graphics);
            foreach (var shot in shots) shot.Draw(e.Graphics);
            foreach (var bullet in bullets) bullet.Draw(e.Graphics);
            minoriko.Draw(e.Graphics);
            shizuha.Draw(e.Graphics);
            e.Graphics.DrawImage(imageFrame, 0, 0, imageFrame.Width, imageFrame.Height);
        }
    }

    internal interface ShootingObject
    {
        public void Progress();

        public void Draw(Graphics graphics);
    }

    internal class SoundEffect
    {
        static WaveStream[] seStreams = new WaveFileReader[] {
            new (Properties.Resources.SE0), new(Properties.Resources.SE1), new(Properties.Resources.SE2),new(Properties.Resources.SE3),
            new (Properties.Resources.SE4), new(Properties.Resources.SE5), new(Properties.Resources.SE6),new(Properties.Resources.SE7),
            new (Properties.Resources.SE8), new(Properties.Resources.SE9), new(Properties.Resources.SE10),new(Properties.Resources.SE11)
        };
        static WaveOut[] SEs = new WaveOut[seStreams.Length];

        public SoundEffect()
        {
            for (int i = 0; i < SEs.Length; ++i)
            {
                SEs[i] = new();
                SEs[i].Init(seStreams[i]);
            }
        }

        public void Play(int effectID)
        {
            seStreams[effectID].Position = 0;
            SEs[effectID].Play();
        }
    }

}