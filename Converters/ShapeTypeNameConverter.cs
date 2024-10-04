using drawingApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Shapes;

namespace drawingApp.Converters
{
    public class ShapeTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var shape = value as Shape;
            if (shape == null)
                return "Kształt";

            var typeName = shape.GetType().Name;

            switch (typeName)
            {
                case "Rectangle":
                    return "Prostokąt";
                case "Ellipse":
                    return "Elipsa";
                case "Line":
                    return "Linia";
                case "Circle":
                    return "Koło";
                default:
                    return "Kształt";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
