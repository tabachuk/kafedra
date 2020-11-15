using KafedraApp.Attributes;
using KafedraApp.Extensions;
using KafedraApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

		public ObservableCollection<Subject> Subjects { get; set; }

		public ObservableCollection<Teacher> Teachers { get; set; }

		public ObservableCollection<AcademicStatusInfo> AcademicStatuses { get; set; }

		#endregion

		#region Private methods

		private string GetPath<T>(string key) => $@"{ DataFolder }\{ key }.json";

		private T Read<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
				return File.ReadAllText(path).FromJson<T>();

			Console.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		private async Task<T> ReadAsync<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
				return await Task.Run(() =>
					File.ReadAllText(path).FromJson<T>());

			Console.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		private List<T> ReadList<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
				return File.ReadAllText(path).FromJson<List<T>>();

			Console.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		private async Task<List<T>> ReadListAsync<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
				return await Task.Run(() =>
					File.ReadAllText(path).FromJson<List<T>>());

			Console.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		private void Write<T>(T data, string key = null)
		{
			lock (_locker)
			{
				key = key ?? GetKey<T>();

				if (!Directory.Exists(DataFolder))
					Directory.CreateDirectory(DataFolder);

				File.WriteAllText(GetPath<T>(key), data.ToJson());
			}
		}

		private async Task WriteAsync<T>(T data, string key = null)
		{
			await Task.Run(() => Write(data, key));
		}

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

		private async Task InitSubjectsAsync()
		{
			var subjects = await ReadListAsync<Subject>();

			if (subjects?.Any() == true)
				Subjects = new ObservableCollection<Subject>(subjects);
			else
				Subjects = new ObservableCollection<Subject>();
		}

		private async Task InitTeachersAsync()
		{
			var teachers = await ReadListAsync<Teacher>();

			if (teachers?.Any() == true)
				Teachers = new ObservableCollection<Teacher>(teachers);
			else
				Teachers = new ObservableCollection<Teacher>();

			Teachers.CollectionChanged += OnTeachersChanged;
		}

		private async Task InitMaxHoursAsync()
		{
			var academicStatuses = await ReadListAsync<AcademicStatusInfo>();

			if (academicStatuses == null)
				throw new FileNotFoundException(
					"Файл із вченими ступенями не знайдений. Перевірте його наявність в папці з даними");

			AcademicStatuses = new ObservableCollection<AcademicStatusInfo>(academicStatuses);
			AcademicStatuses.CollectionChanged += OnAcademicStatusesInfoChanged;

			foreach (var status in AcademicStatuses)
			{
				status.PropertyChanged += OnAcademicStatusInfoChanged;
			}
		}

		#endregion

		#region Public methods

		public async Task InitAsync()
		{
			await InitSubjectsAsync();
			await InitTeachersAsync();
			await InitMaxHoursAsync();
		}

		public async Task SaveSubjects()
		{
			await WriteAsync(Subjects);
		}

		#endregion

		#region Event handlers

		private async void OnTeachersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			await WriteAsync(Teachers);
		}

		private async void OnAcademicStatusesInfoChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			await WriteAsync(AcademicStatuses);
		}

		private async void OnAcademicStatusInfoChanged(object sender, PropertyChangedEventArgs e)
		{
			await WriteAsync(AcademicStatuses);
		}

		#endregion
	}
}
