using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
namespace Focusync.Service.CoreBank.OCR
{

    public class Helper
    {
        public static readonly string LICENSE_KEY = "GKNS2-F0803-G8H48-J8M1V-0B2YP-HGF7VMNRF";//"GMPA2-D0P03-G8HJ8-K8N15-0U2XV-FHT8T6YZA";// "EKP72-90303-G8HC8-J8M9N-012PG-6SHJA-HRETCQ838";//"E5QM2-P0E03-GHEJ8-K885N-0I2GZ-NRWQK-VBPMIDF"; 
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <param name="filePath">The filePath to save.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)width / (float)originalWidth;
            float ratioY = (float)height / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, image.PixelFormat);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            //// Get an ImageCodecInfo object that represents the JPEG codec.
            //ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

            //// Create an Encoder object for the Quality parameter.
            //Encoder encoder = Encoder.Quality;

            //// Create an EncoderParameters object. 
            //EncoderParameters encoderParameters = new EncoderParameters(1);

            //// Save the image as a JPEG file with quality level.
            //EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            //encoderParameters.Param[0] = encoderParameter;
            //newImage.Save(filePath, imageCodecInfo, encoderParameters);
            //return destImage;

            //newImage.Save(filePath);
            //return destImage;
            return newImage;
        }
        /// <summary>
        /// Method to get encoder infor for given image format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>image codec info.</returns>
        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        internal static Bitmap ConvertBlackWhiteImage(Bitmap SourceImage)
        {
            using (Graphics gr = Graphics.FromImage(SourceImage)) // SourceImage is a Bitmap object
            {
                var gray_matrix = new float[][] {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0,      0,      0,      1, 0 },
                    new float[] { 0,      0,      0,      0, 1 }
                };

                var ia = new System.Drawing.Imaging.ImageAttributes();
                ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(gray_matrix));
                //ia.SetThreshold(0.8f); // Change this threshold as needed
                var rc = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);
                gr.DrawImage(SourceImage, rc, 0, 0, SourceImage.Width, SourceImage.Height, GraphicsUnit.Pixel, ia);
            }
            //SourceImage.Save(@"C:\Users\Lenovo\Documents\Visual Studio 2017\Projects\CustomerOnBoard\test.jpg");

            return SourceImage;
        }
        internal static void SetContrast(Bitmap bmp, int threshold)
        {
            var lockedBitmap = new LockBitmap(bmp);
            lockedBitmap.LockBits();

            var contrast = Math.Pow((100.0 + threshold) / 100.0, 2);

            for (int y = 0; y < lockedBitmap.Height; y++)
            {
                for (int x = 0; x < lockedBitmap.Width; x++)
                {
                    var oldColor = lockedBitmap.GetPixel(x, y);
                    var red = ((((oldColor.R / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    var green = ((((oldColor.G / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    var blue = ((((oldColor.B / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    if (red > 255) red = 255;
                    if (red < 0) red = 0;
                    if (green > 255) green = 255;
                    if (green < 0) green = 0;
                    if (blue > 255) blue = 255;
                    if (blue < 0) blue = 0;

                    var newColor = Color.FromArgb(oldColor.A, (int)red, (int)green, (int)blue);
                    lockedBitmap.SetPixel(x, y, newColor);
                }
            }
            lockedBitmap.UnlockBits();
        }
    }
    internal class LockBitmap
    {
        readonly Bitmap _source = null;
        IntPtr _iptr = IntPtr.Zero;
        BitmapData _bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this._source = source;
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            try
            {
                // Get width and height of bitmap
                Width = _source.Width;
                Height = _source.Height;

                // get total locked pixels count
                int pixelCount = Width * Height;

                // Create rectangle to lock
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                // get source bitmap pixel format size
                Depth = Image.GetPixelFormatSize(_source.PixelFormat);

                // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                if (Depth != 8 && Depth != 24 && Depth != 32)
                {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                }

                // Lock bitmap and return bitmap data
                _bitmapData = _source.LockBits(rect, ImageLockMode.ReadWrite,
                                             _source.PixelFormat);

                // create byte array to copy pixel values
                int step = Depth / 8;
                Pixels = new byte[pixelCount * step];
                _iptr = _bitmapData.Scan0;

                // Copy data from pointer to array
                System.Runtime.InteropServices.Marshal.Copy(_iptr, Pixels, 0, Pixels.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Unlock bitmap data
        /// </summary>
        public void UnlockBits()
        {
            try
            {
                // Copy data from byte array to pointer
                System.Runtime.InteropServices.Marshal.Copy(Pixels, 0, _iptr, Pixels.Length);

                // Unlock bitmap data
                _source.UnlockBits(_bitmapData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }
    }
}
