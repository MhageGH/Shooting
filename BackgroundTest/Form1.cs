using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace BackgroundTest
{
    public partial class Form1 : Form
    {
        Mat matGround = BitmapConverter.ToMat(Resource.Ground);
        Mat matCloud = BitmapConverter.ToMat(Resource.Cloud);
        Bitmap imageSunset = Resource.Sunset;
        static int shrink = 150, groundOffset = 0, cloudOffset = 0, maplesOffset = 0, groundSpeed = 5, cloudSpeed = 8, trimWidth = 500, trimHeight = 500;
        Maple[] maples = new Array[10].Select(maple => new Maple()).ToArray();

        public Form1()
        {
            Cv2.VConcat(matGround, matGround, matGround);
            Cv2.VConcat(matCloud, matCloud, matCloud);
            for (int i = 0; i < maples.Length; i++) maples[i] = new Maple();
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            groundOffset = (groundOffset + groundSpeed) % (matGround.Height / 2);
            cloudOffset = (cloudOffset + cloudSpeed) % (matCloud.Height / 2);
            foreach (var maple in maples) maple.Progress();
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var inPoints = new Point2f[] { new(0, 0), new(trimWidth, 0), new(trimWidth, trimHeight), new(0, trimHeight) };
            var outPoints = new Point2f[] { new(shrink, 0), new(trimWidth - shrink, 0), new(trimWidth, trimHeight), new(0, trimHeight) };
            var perspectivMat = Cv2.GetPerspectiveTransform(inPoints, outPoints);
            var maplesBitmap = CreateMaplesBitmap();
            var mats = new Mat[] { matGround, BitmapConverter.ToMat(maplesBitmap), matCloud };
            var offsets = new int[] { groundOffset, maplesOffset, cloudOffset };
            for (int i = 0; i < mats.Length; i++)
            {
                var trimRect = new Rect(0, mats[i].Height - offsets[i] - trimHeight, trimWidth, trimHeight);
                var img = mats[i].Clone(trimRect).WarpPerspective(perspectivMat, trimRect.Size)
                .Clone(new Rect(shrink, 0, trimRect.Width - 2 * shrink, trimRect.Height))
                    .Resize(new OpenCvSharp.Size(387, 451)).ToBitmap();
                e.Graphics.DrawImage(img, 35, 16, img.Width, img.Height);
            }
            e.Graphics.DrawImage(imageSunset, 35, 16, imageSunset.Width, imageSunset.Height);
        }

        Bitmap CreateMaplesBitmap()
        {
            var bitmap = new Bitmap(trimWidth, trimHeight, PixelFormat.Format32bppArgb); // 透明なBitmapキャンバスを作り、そこにGraphicsで書き込んでいく
            var g = Graphics.FromImage(bitmap);
            for (int j = 0; j < maples.Length; j++) maples[j].Draw(g);
            g.Dispose();
            return bitmap;
        }

        class Maple
        {
            Mat matMaple = BitmapConverter.ToMat(Resource.Maple);
            const float max_angular_speed = 0.05f;
            Point2f pos = new(20, 20);
            float[] angles = new float[3];
            float[] angular_speeds = new float[3];
            int width = 50, height = 50;
            float speed = 6;

            public Maple()
            {
                var random = new Random();
                pos.X = trimWidth * random.NextSingle();
                pos.Y = trimHeight * random.NextSingle();
                for (int i = 0; i < angles.Length; i++)
                {
                    angles[i] = 2.0f * MathF.PI * random.NextSingle();
                    angular_speeds[i] = max_angular_speed * random.NextSingle();
                }
            }

            public void Progress()
            {
                pos.Y = (pos.Y + speed) % (16 + 451);
                for (int j = 0; j < angles.Length; j++) angles[j] = (angles[j] + angular_speeds[j]) % (2 * MathF.PI);
            }

            public void Draw(Graphics g)
            {
                var inPoints = new Point2f[] { new(0, 0), new(matMaple.Width, 0), new(matMaple.Width, matMaple.Height), new(0, matMaple.Height) };
                var center = new Point3f(matMaple.Width / 2, matMaple.Height / 2, 0);
                var outPoints = new Point2f[4];
                for (int i = 0; i < inPoints.Length; i++)
                {
                    var p = new Point3f(inPoints[i].X, inPoints[i].Y, 0);
                    p = Rotate(p, center, angles[0], angles[1], angles[2]);
                    outPoints[i] = new Point2f(p.X, p.Y);
                }
                var transMat = Cv2.GetPerspectiveTransform(inPoints, outPoints);
                var image = matMaple.WarpPerspective(transMat, matMaple.Size()).Resize(new OpenCvSharp.Size(width, height)).ToBitmap();
                g.DrawImage(image, pos.X, pos.Y, image.Width, image.Height);

            }

            Point3f Rotate(Point3f point, Point3f center, float angleX, float angleY, float angleZ)
            {
                var p0 = point - center;
                Point3f p1, p2, p3;
                p1.X = p0.X;
                p1.Y = p0.Y * MathF.Cos(angleX) - p0.Z * MathF.Sin(angleX);
                p1.Z = p0.Y * MathF.Sin(angleX) + p0.Z * MathF.Cos(angleX);
                p2.X = p1.Z * MathF.Sin(angleY) + p1.X * MathF.Cos(angleY);
                p2.Y = p1.Y;
                p2.Z = p1.Z * MathF.Cos(angleY) - p1.X * MathF.Sin(angleY);
                p3.X = p2.X * MathF.Cos(angleZ) - p2.Y * MathF.Sin(angleZ);
                p3.Y = p2.X * MathF.Sin(angleZ) + p2.Y * MathF.Cos(angleZ);
                p3.Z = p2.Z;
                return p3 + center;
            }
        }
    }
}