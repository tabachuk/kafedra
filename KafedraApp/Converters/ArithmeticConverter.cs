using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace KafedraApp.Converters
{
	public class ArithmeticConverter : IMultiValueConverter
	{
		public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(parameter is string operation))
				return null;

			var numbers = value.Select(x => System.Convert.ToDouble(x)).ToArray();

			switch (operation)
			{
				case "+":
					return numbers.Sum();
				case "-":
					return numbers.Aggregate(0d, (x, y) => x - y);
				case "*":
					return numbers.Aggregate(1d, (x, y) => x * y);
				case "/":
					return numbers.Aggregate(1d, (x, y) => x / y);
			}

			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
