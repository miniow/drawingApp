using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace drawingApp.Models
{
    public abstract class ImageLoader
    {
        public abstract DrawableImage Load(string filePath);
    }

    public class Ppm3ImageLoader : ImageLoader
    {
        public override DrawableImage Load(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                // Read header, skipping comments
                string magicNumber = ReadNextToken(reader);
                if (magicNumber != "P3") throw new InvalidDataException("Invalid PPM P3 file");

                int width = int.Parse(ReadNextToken(reader));
                int height = int.Parse(ReadNextToken(reader));
                int maxColorValue = int.Parse(ReadNextToken(reader));

                var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Rgb24, null);
                byte[] pixels = new byte[width * height * 3];

                int pixelIndex = 0;
                while (pixelIndex < pixels.Length)
                {
                    string token = ReadNextToken(reader);
                    if (token == null) break;
                    pixels[pixelIndex++] = (byte)(int.Parse(token) * 255 / maxColorValue);
                }

                bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 3, 0);
                return new DrawableImage { Bitmap = bitmap };
            }
        }

        private string ReadNextToken(StreamReader reader)
        {
            string token = "";
            while (true)
            {
                int ch = reader.Read();
                if (ch == -1)
                {
                    return token.Length > 0 ? token : null;
                }

                char character = (char)ch;

                if (character == '#')
                {
                    // Skip comments
                    reader.ReadLine();
                    continue;
                }

                if (char.IsWhiteSpace(character))
                {
                    if (token.Length > 0)
                        return token;
                    continue;
                }

                token += character;
            }
        }
    }

    public class Ppm6ImageLoader : ImageLoader
    {
        public override DrawableImage Load(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fileStream))
            {
                // Read header, skipping comments
                string magicNumber = ReadNextToken(reader);
                if (magicNumber != "P6") throw new InvalidDataException("Invalid PPM P6 file");

                int width = int.Parse(ReadNextToken(reader));
                int height = int.Parse(ReadNextToken(reader));
                int maxColorValue = int.Parse(ReadNextToken(reader));

                int bytesPerColor = maxColorValue < 256 ? 1 : 2;

                long pixelDataSize = (long)width * height * 3 * bytesPerColor;
                if (pixelDataSize > int.MaxValue)
                    throw new InvalidDataException("Image is too large.");

                byte[] pixelData = reader.ReadBytes((int)pixelDataSize);
                if (pixelData.Length != pixelDataSize)
                    throw new InvalidDataException("Incomplete pixel data in PPM P6 file");

                byte[] pixels = new byte[width * height * 3];
                int pixelIndex = 0;

                if (bytesPerColor == 1)
                {
                    // Scale colors when maxColorValue < 256
                    for (int i = 0; i < pixelData.Length; i++)
                    {
                        pixels[pixelIndex++] = (byte)(pixelData[i] * 255 / maxColorValue);
                    }
                }
                else
                {
                    // Scale colors when maxColorValue >= 256
                    for (int i = 0; i < pixelData.Length; i += 2)
                    {
                        int colorValue = (pixelData[i] << 8) | pixelData[i + 1];
                        pixels[pixelIndex++] = (byte)(colorValue * 255 / maxColorValue);
                    }
                }

                var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Rgb24, null);
                bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 3, 0);
                return new DrawableImage { Bitmap = bitmap };
            }
        }

        private string ReadNextToken(BinaryReader reader)
        {
            List<byte> tokenBytes = new List<byte>();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte b = reader.ReadByte();
                char ch = (char)b;

                if (ch == '#')
                {
                    // Skip comments
                    while (b != '\n' && reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        b = reader.ReadByte();
                    }
                    continue;
                }

                if (char.IsWhiteSpace(ch))
                {
                    if (tokenBytes.Count > 0)
                        return Encoding.ASCII.GetString(tokenBytes.ToArray());
                    continue;
                }

                tokenBytes.Add(b);
            }

            return tokenBytes.Count > 0 ? Encoding.ASCII.GetString(tokenBytes.ToArray()) : null;
        }
    }

    public class JpgImageLoader : ImageLoader
    {
        public override DrawableImage Load(string filePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return new DrawableImage { Bitmap = bitmap };
        }
    }
    public class TifImageLoader : ImageLoader
    {
        public override DrawableImage Load(string filePath)
        {
            try
            {
                var bitmap = new BitmapImage(new Uri(filePath));
                return new DrawableImage
                {
                    Bitmap = bitmap,
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load TIFF image: {ex.Message}");
            }
        }
    }

    public class EmptyImageLoader : ImageLoader
    {
        public override DrawableImage Load(string filePath)
        {
            throw new ArgumentOutOfRangeException("Unsupported file type or format");
        }
    }

    public class DrawableImage
    {
        public BitmapSource Bitmap { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class JpgImageSaver
    {
        public void Save(string filePath, BitmapSource bitmap, int qualityLevel)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            if (qualityLevel < 1 || qualityLevel > 100)
                throw new ArgumentOutOfRangeException(nameof(qualityLevel), "Quality level must be between 1 and 100");

            // Convert the bitmap to Bgr24 if it's not already in that format
            BitmapSource formattedBitmap = bitmap;
            if (bitmap.Format != PixelFormats.Bgr24)
            {
                formattedBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgr24, null, 0);
            }

 
            var encoder = new JpegBitmapEncoder
            {
                QualityLevel = qualityLevel
            };

            // Add the formatted bitmap frame to the encoder
            encoder.Frames.Add(BitmapFrame.Create(formattedBitmap));

            // Save the encoded image to the specified file path
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(fileStream);
            }
        }
    }

    public static class ImageLoaderFactory
    {
        public static ImageLoader GetLoaderForFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".ppm" => GetPpmLoader(filePath),
                ".jpg" or ".jpeg" => new JpgImageLoader(),
                ".tif" or ".tiff" => new TifImageLoader(),
                _ => new EmptyImageLoader()
            };
        }

        private static ImageLoader GetPpmLoader(string filePath)
        {
            using var reader = new StreamReader(filePath);
            string magicNumber = reader.ReadLine();
            return magicNumber switch
            {
                "P3" => new Ppm3ImageLoader(),
                "P6" => new Ppm6ImageLoader(),
                _ => new EmptyImageLoader()
            };
        }
    }
}
