using NAudio.Wave;
using System.Linq;
using System.Windows.Input;

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
        static List<Effect> effects = new ();
        static BackGround backGround = new();
        static Minoriko minoriko = new(shots, soundEffect, effects);
        static Shizuha shizuha = new(minoriko, soundEffect, bullets, effects);
        static ShootingObject[] shootingObjects = new ShootingObject[] { backGround, minoriko, shizuha };
        Image imageStar = Properties.Resources.Star;
        Image imagePowerBar = Properties.Resources.PowerBar;
        Image imageShizuhaName = Properties.Resources.ShizuhaName;
        bool pause = false;
        int keyDeadTime = 0;
        int bulletClearTime = 0;

        public Form1()
        {
            InitializeComponent();
            bgm.Init(bgmStream);
            bgm.Volume = 0.5f; // NAudioのボリュームはstaticクラスに結びついているため全WaveOutオブジェクトに適用される。個別の音量調整は出来ない。音源の方で予め比率を調整しておく。
            bgm.Play();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Invalidate();
            backGround.spell = shizuha.spell;
            if (bgmStream.Position == bgmStream.Length)
            {
                bgmStream.Position = 0;
                bgm.Play();
            }
            if (minoriko.life == 0) return;
            if (keyDeadTime > 0) keyDeadTime--;
            if (Keyboard.IsKeyDown(Key.Escape) && keyDeadTime == 0)
            {
                if (pause)
                {
                    pause = false;
                    keyDeadTime = 10;
                }
                else
                {
                    pause = true;
                    keyDeadTime = 10;
                    soundEffect.Play(3);
                }
                return;
            }
            if (pause) return;
            foreach (var shootingObject in shootingObjects) shootingObject.Progress();
            foreach (var shot in shots)
            {
                shot.Progress();
                if (shizuha.enable && (shot.position - shizuha.position).Length() < shot.radius + shizuha.radius)
                {
                    shot.enable = false;
                    shizuha.ReceiveDamage();
                    break;
                }
            }
            shots.RemoveAll(s => s.enable == false);
            if (bulletClearTime > 0) bulletClearTime--;
            if (0 < bulletClearTime && bulletClearTime <= 4)
            {
                for (int i = 0; i < bullets.Count / bulletClearTime; ++i) // ピチュンエフェクトの後、全弾の1/4, 1/3, 1/2, 1/1と徐々に消していく
                {
                    effects.Add(new Effect3(bullets[i].position));
                    bullets[i].enable = false;
                }
            }
            foreach (var bullet in bullets)
            {
                bullet.Progress();
                if ((bullet.position - minoriko.position).Length() < bullet.radius + minoriko.radius && !minoriko.invincible)
                {
                    bulletClearTime = 10;
                    minoriko.Die();
                    break;
                }
            }
            bullets.RemoveAll(b => b.enable == false);
            foreach (var effect in effects) effect.Progress();
            effects.RemoveAll(e => e.enable == false);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var rect = e.ClipRectangle;
            var canvas = new Bitmap(rect.Width, rect.Height);
            var graphics = Graphics.FromImage(canvas);

            // 注意: DrawImage(Image, Point)は元の物理サイズが適用されるのでNG。WidthとHeightを指定すること。
            backGround.Draw(canvas);
            minoriko.Draw(canvas);
            foreach (var shot in shots) shot.Draw(canvas);
            foreach (var bullet in bullets) bullet.Draw(canvas);
            if (shizuha.enable)
            {
                shizuha.Draw(canvas);
                graphics.DrawImage(imageShizuhaName, 45, 16, imageShizuhaName.Width, imageShizuhaName.Height);
                graphics.DrawImage(imagePowerBar, new Rectangle(45, 31, imagePowerBar.Width * shizuha.power / shizuha.power_max, imagePowerBar.Height),
                    new Rectangle(0, 0, imagePowerBar.Width * shizuha.power / shizuha.power_max, imagePowerBar.Height), GraphicsUnit.Pixel);
            }
            //foreach (var effect in effects) effect.Draw(canvas);
            graphics.DrawImage(imageFrame, 0, 0, imageFrame.Width, imageFrame.Height);
            for (int i = 0; i < minoriko.life - 1; ++i) graphics.DrawImage(imageStar, 515 + 15 * i, 108, imageStar.Width, imageStar.Height);
            //shizuha.DrawTalkEvent(canvas);
            if (minoriko.life == 0)
            {
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), 
                    BackGround.position.X, BackGround.position.Y, BackGround.screen_size.Width, BackGround.screen_size.Height);
                graphics.DrawString("Game Over", new Font("メイリオ", 20), Brushes.White, 150, 220);
            }
            else if (shizuha.gameClear)
            {
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)),
                    BackGround.position.X, BackGround.position.Y, BackGround.screen_size.Width, BackGround.screen_size.Height);
                graphics.DrawString("Game Clear!", new Font("メイリオ", 35), Brushes.Gold, 80, 200);
            }
            else if (pause)
            {
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)),
                    BackGround.position.X, BackGround.position.Y, BackGround.screen_size.Width, BackGround.screen_size.Height);
                graphics.DrawString("Pause", new Font("メイリオ", 20), Brushes.White, 180, 220);
            }
            e.Graphics.DrawImage(canvas, 0, 0, canvas.Width, canvas.Height);
        }
    }

    internal interface ShootingObject
    {
        public void Progress();

        public void Draw(Bitmap canvas);
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