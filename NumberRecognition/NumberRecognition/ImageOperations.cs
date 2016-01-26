namespace NumberRecognition
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
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

        public IEnumerable<byte[]> GetImagesBytesForDigit(int num)
        {
            var path = $"images/{num}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
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

                var bytes = GetImageBytes(wbm);
                yield return bytes;
                
            }
        }

        public byte[] GetImageBytes(WriteableBitmap wbm)
        {
            var bytes = new List<byte>();
            wbm.ForEach(
                (x, y, color) =>
                {
                    var one = color.R < 20 || color.G < 20 || color.R < 20;
                    bytes.Add((byte)(one ? 1 : 0));
                    return color;
                });
            
            return bytes.ToArray();
        }
    }
}
