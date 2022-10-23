using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

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
        Mat matmaple = BitmapConverter.ToMat(Properties.Resources.Maple);
        int shrink = 150, groundOffset = 0, cloudOffset = 0, groundSpeed = 5, cloudSpeed = 8, trimWidth = 500, trimHeight = 500;
        System.Drawing.Point position = new System.Drawing.Point(35, 16);
        OpenCvSharp.Size screen_size = new OpenCvSharp.Size(387, 451);

        public NormalBackGround()
        {
            Cv2.VConcat(matGround, matGround, matGround);
            Cv2.VConcat(matCloud, matCloud, matCloud);
        }

        public void Progress()
        {
            groundOffset = (groundOffset + groundSpeed) % (matGround.Height / 2);
            cloudOffset = (cloudOffset + cloudSpeed) % (matCloud.Height / 2);
        }

        public void Draw(Graphics graphics)
        {
            var inPoints = new Point2f[] { new Point2f(0, 0), new Point2f(trimWidth, 0), new Point2f(trimWidth, trimHeight), new Point2f(0, trimHeight) };
            var outPoints = new Point2f[] { new Point2f(shrink, 0), new Point2f(trimWidth - shrink, 0), new Point2f(trimWidth, trimHeight), new Point2f(0, trimHeight) };
            var transMat = Cv2.GetPerspectiveTransform(inPoints, outPoints);
            var mats = new Mat[] { matGround, matCloud };
            var offsets = new int[] { groundOffset, cloudOffset };
            for (int i = 0; i < mats.Length; i++)
            {
                var trimRect = new Rect(0, mats[i].Height - offsets[i] - trimHeight, trimWidth, trimHeight);
                var img = mats[i].Clone(trimRect).WarpPerspective(transMat, trimRect.Size)
                    .Clone(new Rect(shrink, 0, trimRect.Width - 2 * shrink, trimRect.Height)).Resize(screen_size).ToBitmap();
                graphics.DrawImage(img, position.X, position.Y, img.Width, img.Height);
            }
            graphics.DrawImage(imageSunset, position.X, position.Y, imageSunset.Width, imageSunset.Height);
        }
    }
}
