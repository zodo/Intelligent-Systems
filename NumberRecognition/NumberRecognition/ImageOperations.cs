namespace NumberRecognition
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Point = System.Windows.Point;

    public class ImageOperations
    {
        public WriteableBitmap CropResize(WriteableBitmap bmp, Bounds bounds)
        {
            bmp = bmp.Crop(new Rect(new Point(bounds.Left - 5, bounds.Top - 5), new Point(bounds.Right + 5, bounds.Bottom + 5)));
            return bmp.Resize(20, 26, WriteableBitmapExtensions.Interpolation.Bilinear);
        }

        public void SaveImage(WriteableBitmap bmp, int num, string name = null)
        {
            if (!Directory.Exists("images"))
            {
                Directory.CreateDirectory("images");
            }
            if (!Directory.Exists($"images/{num}"))
            {
                Directory.CreateDirectory($"images/{num}");
            }
            var filename = $"images/{num}/{name ?? Guid.NewGuid().ToString()}.png";
            using (FileStream stream5 = new FileStream(filename, FileMode.Create))
            {
                PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                encoder5.Frames.Add(BitmapFrame.Create(bmp));
                encoder5.Save(stream5);
                stream5.Close();
            }
        }

        public IEnumerable<double[]> GetImagesBytesForDigit(int num)
        {
            var path = $"images/{num}";
            if (!Directory.Exists(path))
            {
                yield break;
            }
            var files = Directory.GetFiles(path).Select(Path.GetFullPath).ToList();
            foreach (var file in files)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(file);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                img.EndInit();

                var wbm = new WriteableBitmap(img);

                var vector = BitmapToVector(wbm);
                yield return vector;
                
            }
        }

        public double[] BitmapToVector(WriteableBitmap wbm)
        {
            var vector = new List<double>();
            wbm.ForEach(
                (x, y, color) =>
                {
                    var one = color.R < 20 || color.G < 20 || color.R < 20;
                    vector.Add(one ? 1 : 0);
                    return color;
                });
            
            return vector.ToArray();
        }

        public void DrawEllipses(WriteableBitmap bitmap, int x0, int x1, int y0, int y1)
        {
            if (x0 > x1)
            {
                var t = x0;
                x0 = x1;
                x1 = t;
            }
            if (y0 > y1)
            {
                var t = y0;
                y0 = y1;
                y1 = t;
            }
            int deltax = Math.Abs(x1 - x0);
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int deltaerr = deltay;
            int y = y0;
            for (int x = x0; x < x1; x++)
            {
                bitmap.FillEllipseCentered(x, y, 4, 4, Colors.Black);

                error = error + deltaerr;
                if (2 * error >= deltax)
                {
                    y = y - 1;
                    error = error - deltax;
                }
            }
        }
    }
}
