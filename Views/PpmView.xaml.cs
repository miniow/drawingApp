using drawingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace drawingApp.Views
{
    /// <summary>
    /// Logika interakcji dla klasy PpmView.xaml
    /// </summary>
    public partial class PpmView : UserControl
    {
        private PpmViewModel ViewModel => DataContext as PpmViewModel;
        public PpmView()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Obrazy PPM (*.ppm)|*.ppm|Obrazy JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Wszystkie pliki (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ViewModel.LoadImage(filePath);
            }
        }
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel.CurrentImage != null)
            {
                var position = e.GetPosition((IInputElement)sender);
                int x = (int)(position.X * (ViewModel.CurrentImage.Bitmap.PixelWidth / ((FrameworkElement)sender).ActualWidth));
                int y = (int)(position.Y * (ViewModel.CurrentImage.Bitmap.PixelHeight / ((FrameworkElement)sender).ActualHeight));

                if (x >= 0 && y >= 0 && x < ViewModel.CurrentImage.Bitmap.PixelWidth && y < ViewModel.CurrentImage.Bitmap.PixelHeight)
                {
                    var pixelData = new byte[3];
                    ViewModel.CurrentImage.Bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixelData, 3, 0);
                    ViewModel.ImageDetails = $"Pixel at ({x},{y}): R={pixelData[0]}, G={pixelData[1]}, B={pixelData[2]}";
                }
            }
        }

        //private void SaveAsJpeg_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ViewModel.Images.Count == 0)
        //    {
        //        MessageBox.Show("Brak obrazów do zapisania.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }

        //    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        //    {
        //        Filter = "JPEG Image|*.jpg;*.jpeg",
        //        DefaultExt = "jpg"
        //    };

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        string filePath = saveFileDialog.FileName;
        //        int qualityLevel = (int)jpegQualitySlider.Value;

        //        try
        //        {
        //            // Zakładam, że chcesz zapisać pierwszy obraz z listy
        //            ViewModel.SaveAsJpeg(filePath, qualityLevel);
        //            MessageBox.Show("Obraz został zapisany pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Błąd podczas zapisywania obrazu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}
    }
}
