using System;
using System.Globalization;
using System.Windows.Data;

namespace KafedraApp.Converters
{
	public class MultiplyConverter : IMultiValueConverter
	{
		public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
		{
			var res = (int)(System.Convert.ToDouble(value[0]) * System.Convert.ToDouble(value[1]));

			if (parameter != null && parameter is string stringFormat)
			{
				return stringFormat.Replace("(X)", res.ToString());
			}

			return res;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
