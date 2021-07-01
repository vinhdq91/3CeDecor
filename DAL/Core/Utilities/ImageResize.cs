using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace DAL.Core.Utilities
{
    public static class ImageResize
    {
        public static Bitmap ResizeImage(string imgName, int width, int height)
        {
            Image image = Image.FromFile("wwwroot/Resource/AllImages/" + imgName);
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Bitmap CropImage(string Img, Bitmap bmp, int X, int Y)
        {
            try
            {
                using (Image OriginalImage = Image.FromFile(Img))
                {
                    bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);

                    using (Graphics Graphic = Graphics.FromImage(bmp))
                    {
                        Graphic.SmoothingMode = SmoothingMode.AntiAlias;

                        Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        Graphic.DrawImage(OriginalImage, new Rectangle(0, 0, bmp.Width, bmp.Height), X, Y, bmp.Width, bmp.Width, GraphicsUnit.Pixel);

                        MemoryStream ms = new MemoryStream();

                        bmp.Save(ms, OriginalImage.RawFormat);

                        return bmp;

                    }
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }

        public static void SaveImgToFolder(string folderPath, string imgName, Bitmap fileImage)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string imagePath = folderPath + "/" + imgName;
            if (!File.Exists(imagePath))
            {
                MemoryStream ms = new MemoryStream();
                fileImage.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                var base64StringData = Convert.ToBase64String(byteImage); // Get Base64
                string cleandata = base64StringData.Replace("data:image/png;base64,", "");
                byte[] data = Convert.FromBase64String(cleandata);
                MemoryStream msOutput = new MemoryStream(data);
                Image img = Image.FromStream(msOutput);
                img.Save(imagePath, ImageFormat.Jpeg);
            }
        }
    }
}
