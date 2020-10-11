using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KafedraApp.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter is bool invert && invert)
			{
				return (!(bool)value) ? Visibility.Visible : Visibility.Collapsed;
			}

			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
