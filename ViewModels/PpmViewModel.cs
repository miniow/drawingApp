using drawingApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace drawingApp.ViewModels
{
    public class PpmViewModel : ViewModelBase
    {
        private DrawableImage _currentImage;
        private ObservableCollection<DrawableImage> _images;
        private string _imageDetails;
        private double _zoomFactor = 1.0;
        private string _currentMousePosition;
        private int _jpegQuality = 90; // Default JPEG quality is 90

        public ICommand LoadImageCommand { get; }
        public ICommand SaveImageCommand { get; }

        public DrawableImage CurrentImage
        {
            get => _currentImage;
            set { _currentImage = value; OnPropertyChanged(nameof(CurrentImage)); CommandManager.InvalidateRequerySuggested(); }
        }

        public string ImageDetails
        {
            get => _imageDetails;
            set { _imageDetails = value; OnPropertyChanged(nameof(ImageDetails)); }
        }

        public double ZoomFactor
        {
            get => _zoomFactor;
            set { _zoomFactor = value; OnPropertyChanged(nameof(ZoomFactor)); }
        }

        public string CurrentMousePosition
        {
            get => _currentMousePosition;
            set { _currentMousePosition = value; OnPropertyChanged(nameof(CurrentMousePosition)); }
        }

        // Property for JPEG quality level
        public int JpegQuality
        {
            get => _jpegQuality;
            set
            {
                _jpegQuality = value;
                OnPropertyChanged(nameof(JpegQuality));
            }
        }

        public PpmViewModel()
        {
           
            LoadImageCommand = new RelayCommand(LoadImageExecute);
            SaveImageCommand = new RelayCommand(SaveImageExecute);
        }

        private void LoadImageExecute(object obj)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.ppm, *.jpg)|*.ppm;*.jpg"
            };

            if (dialog.ShowDialog() == true)
            {
                LoadImage(dialog.FileName);
            }
        }



        private void SaveImageExecute(object obj)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JPEG Image (*.jpg)|*.jpg"
            };

            if (dialog.ShowDialog() == true)
            {
                SaveAsJpeg(dialog.FileName, JpegQuality); // Use selected JPEG quality
            }
        }

        public void LoadImage(string filePath)
        {
            ImageLoader loader = GetLoaderForFile(filePath);
            try
            {
                DrawableImage image = loader.Load(filePath);
            
                CurrentImage = image;
                ImageDetails = $"Loaded: {filePath}\nResolution: {image.Bitmap.PixelWidth} x {image.Bitmap.PixelHeight}";
            }
            catch (Exception ex)
            {
                ImageDetails = $"Error loading image: {ex.Message}";
            }
        }

        public void SaveAsJpeg(string filePath, int qualityLevel)
        {
            if (CurrentImage == null)
            {
                ImageDetails = "No image to save.";
                return;
            }

            var image = CurrentImage.Bitmap;
            var saver = new JpgImageSaver();
            try
            {
                saver.Save(filePath, image, qualityLevel);
                ImageDetails = $"Image saved as: {filePath} with quality level {qualityLevel}.";
            }
            catch (Exception ex)
            {
                ImageDetails = $"Error saving image: {ex.Message}";
            }
        }

        private ImageLoader GetLoaderForFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".ppm")
            {
                using (var reader = new StreamReader(filePath))
                {
                    string magicNumber = reader.ReadLine();
                    if (magicNumber == "P3")
                        return new Ppm3ImageLoader();
                    else if (magicNumber == "P6")
                        return new Ppm6ImageLoader();
                    else
                        return new EmptyImageLoader();
                }
            }
            else if (extension == ".jpg" || extension == ".jpeg")
            {
                return new JpgImageLoader();
            }

            return new EmptyImageLoader();
        }

        public void UpdateMousePosition(Point mousePosition)
        {
            if (CurrentImage != null && CurrentImage.Bitmap != null)
            {
                // Adjust the mouse coordinates based on zoom
                int x = (int)(mousePosition.X / ZoomFactor);
                int y = (int)(mousePosition.Y / ZoomFactor);

                // Ensure that x and y are within image bounds
                if (x >= 0 && x < CurrentImage.Bitmap.PixelWidth && y >= 0 && y < CurrentImage.Bitmap.PixelHeight)
                {
                    // Get the pixel color
                    var pixelColor = GetPixelColor(CurrentImage.Bitmap, x, y);
                    CurrentMousePosition = $"X: {x}, Y: {y}, Color: {pixelColor}";
                }
                else
                {
                    CurrentMousePosition = "Out of bounds";
                }
            }
        }

        private Color GetPixelColor(BitmapSource bitmap, int x, int y)
        {
            // Create an array to hold the pixel data
            byte[] pixels = new byte[3]; // For 24bpp images (BGR)

            // Copy the pixel data at (x, y)
            bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 3, 0);

            // Convert the pixel data to a Color object
            return Color.FromRgb(pixels[2], pixels[1], pixels[0]); // BGR to RGB
        }
    }
}
