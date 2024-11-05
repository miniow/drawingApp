using drawingApp.Models;
using drawingApp.Views;
using System;
using System.Collections;
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
    public class ImageCommand
    {
        public BitmapSource ImageState { get; set; }

        public ImageCommand(BitmapSource imageState)
        {
            ImageState = imageState;
        }
    }

    public class PpmViewModel : ViewModelBase
    {
        private Stack<ImageCommand> _undoStack = new Stack<ImageCommand>();
        private Stack<ImageCommand> _redoStack = new Stack<ImageCommand>();

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ShowHistogramCommand { get; }
        public ICommand LoadImageCommand { get; }
        public ICommand SaveImageCommand { get; }
        public ICommand ApplyFilterCommand { get; }

        public ObservableCollection<Filter> Filters { get; set; }
        private Filter _selectedFilter;
        public Filter SelectedFilter
        {
            get => _selectedFilter;
            set { _selectedFilter = value; OnPropertyChanged(nameof(SelectedFilter)); }
        }

        private DrawableImage _currentImage;
        private ObservableCollection<DrawableImage> _images;
        private string _imageDetails;
        private double _zoomFactor = 1.0;
        private string _currentMousePosition;
        private int _jpegQuality = 90;

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
            Filters = new ObservableCollection<Filter> {
new BrightnessFilter(20),
new AddFilter(0,0,0),
new SubtractFilter(0,0,0),
new DivideFilter(0,0,0),
new MultiplyFilter(0,0,0),
new GrayscaleAverageFilter(),
new GrayscaleWeightedFilter(),
new SmoothingFilter(),
new MedianFilter(),
new SobelHorizontalFilter(),
new SobelVerticalFilter(),
new HighPassFilter(),
new LowPassFilter(),
new GaussianBlurFilter(),
new CustomFilter()
};
            UndoCommand = new RelayCommand(UndoExecute, _ => CanUndo());
            RedoCommand = new RelayCommand(RedoExecute, _ => CanRedo());
            LoadImageCommand = new RelayCommand(LoadImageExecute);
            SaveImageCommand = new RelayCommand(SaveImageExecute);
            ApplyFilterCommand = new RelayCommand(ApplySelectedFilter);
            ShowHistogramCommand = new RelayCommand(ShowHistogram);
        }

        private void ShowHistogram(object obj)
        {
            if(CurrentImage == null) return;
            var histogramWindow = new HistogramWindow(this.CurrentImage.Bitmap);

            // Subskrybuj zdarzenie, aby reagować na zmiany obrazu po normalizacji
            histogramWindow.ImageUpdated += OnImageUpdated;

            histogramWindow.Show();
        }

        // Obsługa zdarzenia aktualizacji obrazu
        private void OnImageUpdated(object sender, BitmapSource updatedBitmap)
        {
            // Aktualizujemy obecny obraz w ViewModelu
            this.CurrentImage.Bitmap = updatedBitmap;
            OnPropertyChanged(nameof(CurrentImage));
        }
        private void SaveCurrentState()
        {
            if (CurrentImage?.Bitmap != null)
            {
                _undoStack.Push(new ImageCommand(CurrentImage.Bitmap.Clone()));
                _redoStack.Clear(); // Clear the redo stack on a new operation
            }
        }
        private bool CanUndo() => _undoStack.Count > 0;

        private bool CanRedo() => _redoStack.Count > 0;
        private void ApplySelectedFilter(object obj)
        {
            SaveCurrentState();

            if (CurrentImage?.Bitmap != null && SelectedFilter != null)
            {
                CurrentImage.Bitmap = SelectedFilter.Apply(CurrentImage.Bitmap);
                OnPropertyChanged(nameof(CurrentImage));
            }
            RaiseCanExecuteChanged();
        }
        private void UndoExecute(object obj)
        {
            if (CanUndo())
            {
                var command = _undoStack.Pop();
                _redoStack.Push(new ImageCommand(CurrentImage.Bitmap.Clone()));
                CurrentImage.Bitmap = command.ImageState;
                OnPropertyChanged(nameof(CurrentImage));
                RaiseCanExecuteChanged(); // Update command states after undo
            }
        }

        private void RedoExecute(object obj)
        {
            if (CanRedo())
            {
                var command = _redoStack.Pop();
                _undoStack.Push(new ImageCommand(CurrentImage.Bitmap.Clone()));
                CurrentImage.Bitmap = command.ImageState;
                OnPropertyChanged(nameof(CurrentImage));
                RaiseCanExecuteChanged(); // Update command states after redo
            }
        }
        private void RaiseCanExecuteChanged()
        {
            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
        }
        private void LoadImageExecute(object obj)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.ppm, *.jpg, *.tif)|*.ppm;*.jpg;*.tif"
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
                SaveAsJpeg(dialog.FileName, JpegQuality);
            }
        }

        public void LoadImage(string filePath)
        {
            ImageLoader loader = ImageLoaderFactory.GetLoaderForFile(filePath);
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

        public void UpdateMousePosition(Point mousePosition)
        {
            if (CurrentImage != null && CurrentImage.Bitmap != null)
            {
                int x = (int)(mousePosition.X / ZoomFactor);
                int y = (int)(mousePosition.Y / ZoomFactor);

                if (x >= 0 && x < CurrentImage.Bitmap.PixelWidth && y >= 0 && y < CurrentImage.Bitmap.PixelHeight)
                {
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
            byte[] pixels = new byte[3];
            bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 3, 0);
            return Color.FromRgb(pixels[2], pixels[1], pixels[0]);
        }
    }

}
