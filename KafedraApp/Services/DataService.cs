using KafedraApp.Attributes;
using KafedraApp.Extensions;
using KafedraApp.Models;
using KafedraApp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KafedraApp.Services
{
	public class DataService : IDataService
	{
		#region Fields

		private readonly object _locker = new object();

		#endregion

		#region Properties

		public string DataFolder => "Data";

		public ObservableCollection<Teacher> Teachers { get; set; }

		#endregion

		#region Private methods

		private string GetPath<T>(string key) => $@"{ DataFolder }\{ key }.json";

		private string GetKey<T>()
		{
			string key = null;

			if (typeof(T).GetInterfaces().Any(x => x.Name == "IEnumerable"))
			{
				key = typeof(T).GenericTypeArguments[0]
					.GetCustomAttributes(false)?
					.OfType<CollectionNameAttribute>()?
					.FirstOrDefault()?.CollectionName;
			}
			else if (typeof(T).BaseType == typeof(BaseModel))
			{
				key = typeof(T).GetCustomAttributes(false)?
					.OfType<CollectionNameAttribute>()?
					.FirstOrDefault()?.CollectionName;
			}

			return string.IsNullOrEmpty(key) ? typeof(T).Name : key;
		}

		#endregion

		#region Public methods

		public async Task InitAsync()
		{
			var teachers = await ReadListAsync<Teacher>();

			if (teachers?.Any() == true)
				Teachers = new ObservableCollection<Teacher>(teachers);
			else
				Teachers = new ObservableCollection<Teacher>();

			Teachers.CollectionChanged += OnTeachersChanged;
		}

		public T Read<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
				return File.ReadAllText(path).FromJson<T>();

			Console.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		public async Task<T> ReadAsync<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
				return await Task.Run(() =>
					File.ReadAllText(path).FromJson<T>());

			Console.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		public List<T> ReadList<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
				return File.ReadAllText(path).FromJson<List<T>>();

			Console.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		public async Task<List<T>> ReadListAsync<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
				return await Task.Run(() =>
					File.ReadAllText(path).FromJson<List<T>>());

			Console.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		public void Write<T>(T data, string key = null)
		{
			lock(_locker)
			{
				key = key ?? GetKey<T>();

				if (!Directory.Exists(DataFolder))
					Directory.CreateDirectory(DataFolder);

				File.WriteAllText(GetPath<T>(key), data.ToJson());
			}
		}

		public async Task WriteAsync<T>(T data, string key = null)
		{
			await Task.Run(() => Write(data, key));
		}

		#endregion

		#region Event handlers

		private async void OnTeachersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			await WriteAsync(Teachers);
		}

		#endregion
	}
}
