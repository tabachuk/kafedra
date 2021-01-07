using System;
using System.Globalization;
using System.Windows.Data;

namespace KafedraApp.Converters
{
	public class PercentMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var a = System.Convert.ToDouble(values[0]);
			var b = System.Convert.ToDouble(values[1]);

			if (b == 0)
				return 0;

			return Math.Round(a * 100 / b, 0, MidpointRounding.AwayFromZero);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
