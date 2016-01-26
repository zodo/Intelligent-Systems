using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;


namespace NumberRecognition.Test
{
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    [TestClass]
    public class ImageOperationsTest
    {
        [TestCleanup]
        public void ClearSavedImages()
        {
            if (Directory.Exists("images/42"))
            {
                Directory.Delete("images/42", true);
            }
        }

        [TestMethod]
        public void GetImageBytes_WhiteImage_ReturnsZeroArray()
        {
            // Arrange
            var imgop = new ImageOperations();
            var wbm = BitmapFactory.New(30, 30);
            wbm.Clear(Colors.White);
            // Act
            var bytes = imgop.GetImageBytes(wbm);
            // Assert
            Assert.IsTrue(bytes.All(x => x == 0));
        }

        [TestMethod]
        public void GetImageBytes_BlackImage_ReturnsOnesArray()
        {
            // Arrange
            var imgop = new ImageOperations();
            var wbm = BitmapFactory.New(30, 30);
            wbm.Clear(Colors.Black);
            // Act
            var bytes = imgop.GetImageBytes(wbm);
            // Assert
            Assert.IsTrue(bytes.All(x => x == 1));
        }

        [TestMethod]
        public void SaveImage_Image_ImageSavedCorrectly()
        {
            // Arrange
            var imgop = new ImageOperations();
            var wbm = BitmapFactory.New(30, 30);
            wbm.Clear(Colors.Black);
            // Act
            imgop.SaveImage(wbm, 42, "test");
            var bytes = imgop.GetImagesBytesForDigit(42).First();
            
            // Assert
            Assert.IsTrue(bytes.All(x => x == 1));
        }

    }
}
