using System.Drawing;

namespace Cruder.Helper
{
    public class ImageHelper
    {
        public static Image Crop(Image source, Rectangle cropArea)
        {
            Image retVal;

            using (Bitmap bitmap = new Bitmap(source))
            {
                retVal = bitmap.Clone(cropArea, bitmap.PixelFormat);
            }

            return retVal;
        }

        public static Image Resize(Image source, Size size)
        {
            int width = source.Width;
            int height = source.Height;

            float num3 = 0f;
            float num4 = 0f;
            float num5 = 0f;

            num4 = ((float)size.Width) / ((float)width);
            num5 = ((float)size.Height) / ((float)height);

            if (num5 < num4)
            {
                num3 = num5;
            }
            else
            {
                num3 = num4;
            }

            int num6 = (int)(width * num3);
            int num7 = (int)(height * num3);

            Bitmap image = new Bitmap(num6, num7);

            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, 0, 0, num6, num7);
            }

            return image;
        }
    }
}
