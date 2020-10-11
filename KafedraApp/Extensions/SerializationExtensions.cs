using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KafedraApp.Converters;

namespace KafedraApp.Extensions
{
	public static class SerializationExtensions
	{
		public static string ToJson<T>(this T obj)
		{
			if (obj != null)
			{
				var converter = new BaseModelJsonConverter();

				if (converter.CanConvert(typeof(T)))
				{
					return JsonConvert.SerializeObject(obj, Formatting.Indented, converter);
				}
				else
				{
					return JsonConvert.SerializeObject(obj, Formatting.Indented);
				}
			}

			return null;
		}

		public static T FromJson<T>(this string json)
		{
			if (string.IsNullOrEmpty(json))
				return default;

			var converter = new BaseModelJsonConverter();

			if (converter.CanConvert(typeof(T)))
			{
				return JsonConvert.DeserializeObject<T>(json, converter);
			}
			else
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
		}
	}
}
