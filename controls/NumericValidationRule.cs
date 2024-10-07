using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace drawingApp.Controls
{
    public class NumericValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = (value ?? "").ToString();

            if (double.TryParse(input, out _))
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Wartość musi być liczbą.");
        }
    }
}
