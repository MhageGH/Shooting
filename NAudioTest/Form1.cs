using NAudio.Wave;
namespace NAudioTest
{
    public partial class Form1 : Form
    {
        WaveStream bgmStream = new Mp3FileReader(new System.IO.MemoryStream(Resource.BGM0));
        WaveOut bgm = new();

        public Form1()
        {
            InitializeComponent();
            bgm.Init(bgmStream);
            bgm.Play();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bgmStream.Position == bgmStream.Length)
            {
                bgmStream.Position = 0;
                bgm.Play();
            }
        }
    }
}