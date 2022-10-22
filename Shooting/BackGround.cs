using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Shooting
{
    internal class BackGround
    {
        static Bitmap image = Properties.Resources.Ground;
        Mat src_img = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);
        int shrink = 150;
        int offset = 0;
        int speed = 5;
        int trimHeight = 500;
        OpenCvSharp.Size screen_size = new OpenCvSharp.Size(500, 600);

        public BackGround()
        {
            Cv2.VConcat(src_img, src_img, src_img);
        }

        public void Draw(Graphics graphics)
        {
            offset = (offset + speed) % (src_img.Height / 2);
            var src_img_trimmed = src_img.Clone(new Rect(0, src_img.Height - offset - trimHeight, src_img.Width, trimHeight));
            var inPoints = new Point2f[] 
            {
                new Point2f(0, 0), new Point2f(src_img_trimmed.Width, 0), new Point2f(src_img_trimmed.Width, src_img_trimmed.Height), new Point2f(0, src_img_trimmed.Height) 
            };
            var outPoints = new Point2f[]
            {
                new Point2f(shrink, 0), new Point2f(src_img_trimmed.Width - shrink, 0), new Point2f(src_img_trimmed.Width, src_img_trimmed.Height), new Point2f(0, src_img_trimmed.Height)
            };
            var transMat = Cv2.GetPerspectiveTransform(inPoints, outPoints);
            var dst_img = new Mat(new OpenCvSharp.Size(src_img_trimmed.Width, src_img_trimmed.Height), MatType.CV_8UC3);
            Cv2.WarpPerspective(src_img_trimmed, dst_img, transMat, dst_img.Size());
            dst_img = dst_img.Clone(new Rect(shrink, 0, src_img_trimmed.Width - 2 * shrink, src_img_trimmed.Height));
            Cv2.Resize(dst_img, dst_img, screen_size);
            graphics.DrawImage(dst_img.ToBitmap(), 0, 0);
        }
    }
}
