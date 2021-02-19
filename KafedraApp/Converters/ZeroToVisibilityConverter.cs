using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KafedraApp.Converters
{
	public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			double.TryParse((value ?? "").ToString(), out double val);

			return Math.Abs(val) == 0.0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
