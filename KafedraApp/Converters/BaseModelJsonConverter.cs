using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KafedraApp.Models;

namespace KafedraApp.Converters
{
	public class BaseModelJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.BaseType == typeof(BaseModel)
				|| (objectType.GetInterfaces().Any(x => x.Name == "IEnumerable")
				&& objectType.GenericTypeArguments[0].BaseType == typeof(BaseModel));
		}

		public override object ReadJson(
			JsonReader reader,
			Type objectType,
			object existingValue,
			JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				var item = JObject.Load(reader);
				PopulateIds(item, objectType);
				return item.ToObject(objectType);
			}
			else
			{
				var array = JArray.Load(reader);

				foreach (var jtoken in array)
				{
					PopulateIds(jtoken, objectType.GenericTypeArguments[0]);
				}

				return array.ToObject(objectType);
			}
		}

		public override void WriteJson(
			JsonWriter writer,
			object value,
			JsonSerializer serializer)
		{
			var jtoken = JToken.FromObject(value);

			if (jtoken is JObject jobj)
			{
				WriteIdsToJObject(jobj, value).WriteTo(writer);
			}
			else if (jtoken is JArray jarr)
			{
				writer.WriteStartArray();

				for (int i = 0; i < jarr.Count; ++i)
				{
					var item = (value as IEnumerable<BaseModel>).ElementAt(i);
					WriteIdsToJObject(jarr[i] as JObject, item).WriteTo(writer);
				}

				writer.WriteEndArray();
			}
		}

		private JObject WriteIdsToJObject(JObject jobj, object obj)
		{
			var props = obj?.GetType()?.GetProperties()?
				.Where(x =>
					x?.GetValue(obj) is IEnumerable<BaseModel>
					&& x?.GetCustomAttributes(false)?
						.Any(y => y?.GetType()?.Name == nameof(JsonIgnoreAttribute)) == true);

			if (props?.Any() == true)
			{
				foreach (var prop in props)
				{
					var ids = (prop.GetValue(obj) as IEnumerable<BaseModel>)?
						.Select(x => x?.Id)?.Where(x => !string.IsNullOrEmpty(x));

					if (ids?.Any() == true)
						jobj.Add(new JProperty(prop.Name, new JArray(ids)));
				}
			}

			return jobj;
		}

		private void PopulateIds(JToken jtoken, Type objectType)
		{
			var obj = jtoken.ToObject(objectType);

			var props = obj?.GetType()?.GetProperties()?
				.Where(x => x?.PropertyType?.GenericTypeArguments?.Any() == true
					&& x?.PropertyType?.GenericTypeArguments[0]?.BaseType == typeof(BaseModel)
					&& !Attribute.IsDefined(x, typeof(JsonIgnoreAttribute)));

			if (props?.Any() == true)
			{
				foreach (var prop in props)
				{
					var ids = jtoken[prop.Name].ToArray();
					prop.SetValue(obj, Activator.CreateInstance(prop.PropertyType));

					foreach (var id in ids)
					{
						var list = prop.GetValue(obj);
						var baseModel = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);
						var idProp = baseModel.GetType().GetProperty("Id");
						idProp.SetValue(baseModel, id.Value<string>());
						prop.PropertyType.GetMethod("Add").Invoke(list, new object[] { baseModel });
					}
				}
			}

			jtoken = JToken.FromObject(obj);
		}
	}
}
