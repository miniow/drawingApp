using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows;
using drawingApp.ViewModels;

namespace drawingApp.Converters
{
    public class ShapeTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ShapeType shapeType && parameter is string expectedType)
            {
                if (expectedType == "RectangleOrEllipse" && (shapeType == ShapeType.Rectangle || shapeType == ShapeType.Ellipse))
                    return Visibility.Visible;
                if (expectedType == "Circle" && shapeType == ShapeType.Circle)
                    return Visibility.Visible;
                if (expectedType == "Line" && shapeType == ShapeType.Line)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
