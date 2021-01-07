using System;
using System.Globalization;
using System.Windows.Data;

namespace KafedraApp.Converters
{
	public class PercentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var a = System.Convert.ToDouble(value);
			var b = System.Convert.ToDouble(parameter);

			if (b == 0)
				return 0;

			return Math.Round(a * 100 / b, 0, MidpointRounding.AwayFromZero);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
