using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Internal.Vectors;
using System.Drawing;
using System.Drawing.Imaging;

namespace Shooting
{
    internal class BackGround
    {
        NormalBackGround normalBackGround = new NormalBackGround();
        SpellBackGround spellBackGround = new SpellBackGround();

        public bool SpellEnable = false;

        public void Progress()
        {
            if (SpellEnable) spellBackGround.Progress();
            else normalBackGround.Progress();
        }

        public void Draw(Graphics graphics)
        {
            if (SpellEnable) spellBackGround.Draw(graphics);
            else normalBackGround.Draw(graphics);
        }
    }

    internal class SpellBackGround
    {
        public void Progress()
        {

        }

        public void Draw(Graphics graphics)
        {
        }
    }

    internal class NormalBackGround
    {
        Bitmap imageSunset = Properties.Resources.Sunset;
        Mat matGround = BitmapConverter.ToMat(Properties.Resources.Ground);
        Mat matCloud = BitmapConverter.ToMat(Properties.Resources.Cloud);
        Mat matMaple = BitmapConverter.ToMat(Properties.Resources.Maple);
        int shrink = 150, groundOffset = 0, cloudOffset = 0, groundSpeed = 5, cloudSpeed = 8, trimWidth = 500, trimHeight = 500;
        static System.Drawing.Point position = new System.Drawing.Point(35, 16);
        static OpenCvSharp.Size screen_size = new OpenCvSharp.Size(387, 451);
        Maple[] maples = new Maple[10];

        public NormalBackGround()
        {
            Cv2.VConcat(matGround, matGround, matGround);
            Cv2.VConcat(matCloud, matCloud, matCloud);
            for (int i = 0; i < maples.Length; i++)
            {
                maples[i] = new Maple();
            }
        }

        public void Progress()
        {
            groundOffset = (groundOffset + groundSpeed) % (matGround.Height / 2);
            cloudOffset = (cloudOffset + cloudSpeed) % (matCloud.Height / 2);
            for (int i = 0; i < maples.Length; i++)
            {
                maples[i].pos.Y = (maples[i].pos.Y + Maple.speed) % (position.Y + screen_size.Height);
                for (int j = 0; j < maples[i].angles.Length; j++)
                {
                    maples[i].angles[j] = (maples[i].angles[j] + maples[i].angular_speeds[j]) % (2 * (float)Math.PI);
                }
            }
        }

        public void Draw(Graphics graphics)
        {
            var inPoints = new Point2f[] { new Point2f(0, 0), new Point2f(trimWidth, 0), new Point2f(trimWidth, trimHeight), new Point2f(0, trimHeight) };
            var outPoints = new Point2f[] { new Point2f(shrink, 0), new Point2f(trimWidth - shrink, 0), new Point2f(trimWidth, trimHeight), new Point2f(0, trimHeight) };
            var perspectivMat = Cv2.GetPerspectiveTransform(inPoints, outPoints);
            var mats = new Mat[] { matGround, matCloud };
            var offsets = new int[] { groundOffset, cloudOffset };
            for (int i = 0; i < mats.Length; i++)
            {
                var trimRect = new Rect(0, mats[i].Height - offsets[i] - trimHeight, trimWidth, trimHeight);
                var img = mats[i].Clone(trimRect).WarpPerspective(perspectivMat, trimRect.Size)
                    .Clone(new Rect(shrink, 0, trimRect.Width - 2 * shrink, trimRect.Height)).Resize(screen_size).ToBitmap();
                graphics.DrawImage(img, position.X, position.Y, img.Width, img.Height);
                if (i == 0)
                {
                    var maplesBitmap = CreateMaplesBitmap(outPoints);    // 地面と雲の間に紅葉が描かれる   // TODO Maplesを大きいMatに一度写してから射影変換した方が良い。
                    graphics.DrawImage(maplesBitmap, position.X, position.Y, maplesBitmap.Width, maplesBitmap.Height);
                }
            }
            graphics.DrawImage(imageSunset, position.X, position.Y, imageSunset.Width, imageSunset.Height);
        }

        Bitmap CreateMaplesBitmap(Point2f[] groundOutPoints)
        {
            var inPoints = new Point2f[] { new Point2f(0, 0), new Point2f(matMaple.Width, 0), new Point2f(matMaple.Width, matMaple.Height), new Point2f(0, matMaple.Height) };
            var center = new Point3f(matMaple.Width / 2, matMaple.Height / 2, 0);
            var outPoints = new Point2f[4];
            var bitmap = new Bitmap(trimWidth, trimHeight, PixelFormat.Format32bppArgb); // 透明なBitmapキャンバスを作り、そこにGraphicsで書き込んでいく
            var g = Graphics.FromImage(bitmap);
            for (int j = 0; j < maples.Length; j++)
            {
                for (int i = 0; i < inPoints.Length; i++)
                {
                    var p = new Point3f(inPoints[i].X, inPoints[i].Y, 0);
                    p = Rotate(p, center, maples[j].angles[0], maples[j].angles[1], maples[j].angles[2]);
                    outPoints[i] = new Point2f(p.X, p.Y);
                }
                var transMat = Cv2.GetPerspectiveTransform(inPoints, outPoints);
                var a = groundOutPoints[1].X - groundOutPoints[0].X;
                var b = groundOutPoints[2].X - groundOutPoints[3].X;
                var h = groundOutPoints[2].Y - groundOutPoints[1].Y;
                var y = maples[j].pos.Y - groundOutPoints[0].Y;
                var c = a + (b - a) * y / h;
                var scale = c / b;
                var centerX = (groundOutPoints[0].X + groundOutPoints[1].X) / 2;
                var posX = centerX + (maples[j].pos.X - centerX) * c / a;
                var image = matMaple.WarpPerspective(transMat, matMaple.Size()).Resize(new OpenCvSharp.Size(Maple.width * scale, Maple.height * scale)).ToBitmap();
                g.DrawImage(image, posX, maples[j].pos.Y, image.Width, image.Height);
            }
            g.Dispose();
            return bitmap;
        }

        class Maple
        {
            const float max_angular_speed = 0.05f;
            public Point2f pos;
            public float[] angles = new float[3];
            public float[] angular_speeds = new float[3];
            static public int width = 70, height = 70;
            static public float speed = 6;

            public Maple()
            {
                var random = new Random();
                pos.X = screen_size.Width * random.NextSingle();
                pos.Y = screen_size.Height * random.NextSingle();
                for (int i = 0; i < angles.Length; i++)
                {
                    angles[i] = 2.0f * (float)Math.PI * random.NextSingle();
                    angular_speeds[i] = max_angular_speed*random.NextSingle();
                }
            }
        }

        Point3f Rotate(Point3f point, Point3f center, float angleX, float angleY, float angleZ)
        {
            var p0 = point - center;
            Point3f p1, p2, p3;
            p1.X = p0.X;
            p1.Y = p0.Y * (float)Math.Cos(angleX) - p0.Z * (float)Math.Sin(angleX);
            p1.Z = p0.Y * (float)Math.Sin(angleX) + p0.Z * (float)Math.Cos(angleX);
            p2.X = p1.Z * (float)Math.Sin(angleY) + p1.X * (float)Math.Cos(angleY);
            p2.Y = p1.Y;
            p2.Z = p1.Z * (float)Math.Cos(angleY) - p1.X * (float)Math.Sin(angleY);
            p3.X = p2.X * (float)Math.Cos(angleZ) - p2.Y * (float)Math.Sin(angleZ);
            p3.Y = p2.X * (float)Math.Sin(angleZ) + p2.Y * (float)Math.Cos(angleZ);
            p3.Z = p2.Z;
            return p3 + center;
        }
    }
}
