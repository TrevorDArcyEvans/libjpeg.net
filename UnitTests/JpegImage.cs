using System.Drawing;
using System.IO;
using NUnit.Framework;
using BitMiracle.LibJpeg;

namespace UnitTests
{
    [TestFixture]
    public class JpegImageTests
    {
        private static string[] DecompressionFiles
        {
            get
            {
                return new string[]
                {
                    "BLU.JPG",
                    "PARROTS.JPG",
                    "3D.JPG",
                    "MARBLES.JPG",
                };
            }
        }

        private static string[] BitmapFiles
        {
            get
            {
                return new string[]
                {
                    "duck.bmp",
                    "particle.bmp",
                    "pink.png",
                    "rainbow.bmp"
                };
            }
        }

        [Test, TestCaseSource("DecompressionFiles")]
        public void TestDecompressionResultsSameAsForDJpeg(string fileName)
        {
            string outputFileName = fileName.Replace(".JPG", ".bmp");
            testBitmapFromFile(Tester.MapOpenPath(fileName), outputFileName);
        }

        [Test]
        public void TestDecompressionFromCMYKJpeg()
        {
            using var fs = File.OpenRead(Tester.MapOpenPath("ammerland.jpg"));
            using (JpegImage jpeg = new JpegImage(fs))
            {
                Assert.AreEqual(jpeg.BitsPerComponent, 8);
                Assert.AreEqual(jpeg.ComponentsPerSample, 4);
                Assert.AreEqual(jpeg.Colorspace, Colorspace.CMYK);
                Assert.AreEqual(jpeg.Width, 315);
                Assert.AreEqual(jpeg.Height, 349);

                testBitmapOutput(jpeg, "ammerland.bmp");
            }
        }

        [Test]
        public void TestGrayscaleJpegToBitmap()
        {
            using var fs = File.OpenRead(Tester.MapOpenPath("turkey.jpg"));
            using (JpegImage jpegImage = new JpegImage(fs))
            {
                testBitmapOutput(jpegImage, "turkey.png");
            }
        }

        [Test]
        public void TestCreateFromPixelsAndRecompress()
        {
            using (JpegImage jpegImage = createImageFromPixels())
            {
                CompressionParameters compressionParameters = new CompressionParameters();
                compressionParameters.Quality = 20;
                const string output = "JpegImageFromPixels_20.jpg";
                testJpegOutput(jpegImage, compressionParameters, output);

                using var fs = File.OpenRead(output);
                using (JpegImage recompressedImage = new JpegImage(fs))
                {
                    Assert.AreEqual(recompressedImage.Colorspace, jpegImage.Colorspace);
                }
            }
        }

        [Test]
        public void TestCreateJpegImageFromPixels()
        {
            using (JpegImage jpegImage = createImageFromPixels())
            {
                testJpegOutput(jpegImage, "JpegImageFromPixels.jpg");
                testBitmapOutput(jpegImage, "JpegImageFromPixels.png");
            }
        }

        private static JpegImage createImageFromPixels()
        {
            byte[] rowData = new byte[96];
            for (int i = 0; i < rowData.Length; ++i)
            {
                if (i < 5)
                    rowData[i] = 0xE4;
                else if (i < 15)
                    rowData[i] = 0xAB;
                else if (i < 35)
                    rowData[i] = 0x00;
                else if (i < 55)
                    rowData[i] = 0x65;
                else
                    rowData[i] = 0xF0;
            }

            const int width = 24;
            const int height = 25;
            const byte bitsPerComponent = 8;
            const byte componentsPerSample = 4;
            const Colorspace colorspace = Colorspace.CMYK;

            SampleRow row = new SampleRow(rowData, width, bitsPerComponent, componentsPerSample);
            SampleRow[] rows = new SampleRow[height];
            for (int i = 0; i < rows.Length; ++i)
                rows[i] = row;

            JpegImage jpegImage = new JpegImage(rows, colorspace);
            Assert.AreEqual(jpegImage.Width, width);
            Assert.AreEqual(jpegImage.Height, rows.Length);
            Assert.AreEqual(jpegImage.BitsPerComponent, bitsPerComponent);
            Assert.AreEqual(jpegImage.ComponentsPerSample, componentsPerSample);
            Assert.AreEqual(jpegImage.Colorspace, colorspace);
            return jpegImage;
        }
        
        private static void testBitmapFromFile(string sourceFileName, string bitmapFileName)
        {
            using var fs = File.OpenRead(sourceFileName);
            using (JpegImage jpeg = new JpegImage(fs))
            {
                testBitmapOutput(jpeg, bitmapFileName);
            }
        }

        private static void testJpegOutput(JpegImage jpeg, string jpegFileName)
        {
            testJpegOutput(jpeg, new CompressionParameters(), jpegFileName);
        }

        private static void testJpegOutput(JpegImage jpeg, CompressionParameters parameters, string jpegFileName)
        {
            using (FileStream output = new FileStream(jpegFileName, FileMode.Create))
                jpeg.WriteJpeg(output, parameters);

            FileAssert.AreEqual(jpegFileName, Tester.MapExpectedPath(jpegFileName));
        }

        private static void testBitmapOutput(JpegImage jpeg, string bitmapFileName)
        {
            using (FileStream output = new FileStream(bitmapFileName, FileMode.Create))
                jpeg.WriteBitmap(output);

            FileAssert.AreEqual(bitmapFileName, Tester.MapExpectedPath(bitmapFileName));
        }
    }
}
