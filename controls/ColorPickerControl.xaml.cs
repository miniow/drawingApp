using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using drawingApp.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace drawingApp.controls
{
    /// <summary>
    /// Logika interakcji dla klasy ColorPickerControl.xaml
    /// </summary>
    public partial class ColorPickerControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty CurrentColorBrushProperty =
    DependencyProperty.Register(
        nameof(CurrentColorBrush),
        typeof(Brush),
        typeof(ColorPickerControl));

        public event PropertyChangedEventHandler PropertyChanged;

        private ColorModel _currentColorModel;
        public ColorModel CurrentColorModel
        {
            get => _currentColorModel;
            set
            {
                _currentColorModel = value;
                OnPropertyChanged(nameof(CurrentColorModel));
                OnPropertyChanged(nameof(CurrentColorBrush));
            }
        }

        public Brush CurrentColorBrush
        {
            get { return (Brush)GetValue(CurrentColorBrushProperty); } 
            set => SetValue(CurrentColorBrushProperty, value);
        }

        public ColorPickerControl()
        {
            InitializeComponent();
            DataContext = this;
            // Domyślnie ustawienie modelu RGB
            CurrentColorModel = new RGBModel();
            UpdateColor();
        }
        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateColor();
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OnColorModelChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    if (selectedItem.Content.ToString() == "RGB")
                    {
                        // Konwersja z CMYK na RGB
                        if (CurrentColorModel is CMYKModel cmykModel)
                        {
                            CurrentColorModel = cmykModel.ToRGBModel();
                        }
                        else
                        {
                            // Jeśli już mamy model RGB, nic nie rób
                            CurrentColorModel = new RGBModel();
                        }
                    }
                    else if (selectedItem.Content.ToString() == "CMYK")
                    {
                        // Konwersja z RGB na CMYK
                        if (CurrentColorModel is RGBModel rgbModel)
                        {
                            CurrentColorModel = rgbModel.ToCMYKModel();
                        }
                        else
                        {
                            // Jeśli już mamy model CMYK, nic nie rób
                            CurrentColorModel = new CMYKModel();
                        }
                    }
                }
            }
        }
        private void UpdateColor()
        {
            if (CurrentColorModel != null)
            {
                var color = CurrentColorModel.ToColor();
                CurrentColorBrush = new SolidColorBrush(color);
            }
        }
    }
    public class ColorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RGBTemplate { get; set; }
        public DataTemplate CMYKTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is RGBModel)
            {
                return RGBTemplate;
            }
            else if (item is CMYKModel)
            {
                return CMYKTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }



}

