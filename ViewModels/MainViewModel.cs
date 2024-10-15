using drawingApp.Models;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace drawingApp.ViewModels
{
    public class ChangeViewEventArgs : EventArgs
    {
        public string ViewName { get; set; }
    }



    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private string _caption;
        private IconChar _icon;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }
        public string Caption { get { return _caption; } set { _caption = value; OnPropertyChanged(nameof(Caption)); } }
        public IconChar Icon { get { return _icon; } set { _icon = value; OnPropertyChanged(nameof(Icon)); } }

        public ICommand ShowDrawViewCommand { get;}
        public ICommand ShowPpmViewCommand { get;}
        public MainViewModel()
        {
            CurrentViewModel = new DrawViewModel();

            ShowDrawViewCommand = new RelayCommand(ExecuteShowDrawViewCommand);
            ShowPpmViewCommand = new RelayCommand(ExecuteShowPpmViewCommand);
        }

        private void ExecuteShowPpmViewCommand(object obj)
        {
            CurrentViewModel = new PpmViewModel();
            Caption = "Ppm";
            Icon = IconChar.File;
        }

        private void ExecuteShowDrawViewCommand(object obj)
        {
            CurrentViewModel = new DrawViewModel();
            Caption = "Drawing";
            Icon = IconChar.DrawPolygon;
        }
    }

}
