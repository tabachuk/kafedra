using KafedraApp.Attributes;
using KafedraApp.Extensions;
using KafedraApp.Models;
using KafedraApp.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KafedraApp.Services
{
	public class DataService : IDataService
	{
		#region Constants

		public const string DefaultDataFolderName = "Data";

		#endregion

		#region Fields

		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly object _locker = new object();

		#endregion

		#region Properties

		public string DataPath { get; private set; }

		public string DefaultDataPath => $@"{Directory.GetCurrentDirectory()}\{DefaultDataFolderName}";

		public ObservableCollection<Subject> Subjects { get; set; }

		public ObservableCollection<Teacher> Teachers { get; set; }

		public ObservableCollection<Group> Groups { get; set; }

		public ObservableCollection<AcademicStatusInfo> AcademicStatuses { get; set; }

		public ObservableCollection<TimeNorm> TimeNorms { get; set; }

		public List<string> SubjectNames =>
			Subjects?
			.Select(x => x.Name)
			.Distinct()
			.Where(x => !x.Contains("робота") && !x.Contains("практика"))
			.OrderBy(x => x)
			.ToList();

		#endregion

		#region Constructors

		public DataService()
		{
			InitDataPath();
		}

		#endregion

		#region Private Methods

		private void InitDataPath()
		{
			if (Directory.Exists(Settings.Default.DataPath))
			{
				DataPath = Settings.Default.DataPath;
			}
			else
			{
				DataPath = DefaultDataPath;

				if (!Directory.Exists(DataPath))
				{
					Directory.CreateDirectory(DataPath);
				}
			}
		}

		private string GetPath(string key, string folder = null)
		{
			return $@"{ folder ?? DataPath }\{ key }.json";
		}

		private string GetPath<T>(string folder = null, string key = null)
		{
			key = key ?? GetKey<T>();

			if (string.IsNullOrEmpty(key))
				throw new Exception("Key is null or empty.");

			return GetPath(key, folder);
		}

		private T Read<T>(string key = null)
		{
			string path = null;

			try
			{
				path = GetPath<T>(key);

				_logger.Info("Started reading '{0}.json'.", path);

				if (File.Exists(path))
				{
					var res = File.ReadAllText(path).FromJson<T>();
					_logger.Info("'{0}.json' has been read.", path);
					return res;
				}

				_logger.Warn("Can't read '{0}.json'. File does not exist.", path);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Can't read '{0}.json'.", path);
			}

			return default;
		}

		private async Task<T> ReadAsync<T>(string key = null)
		{
			string path = null;

			try
			{
				path = GetPath<T>(key);

				_logger.Info("Started reading '{0}'.", path);

				if (File.Exists(path))
				{
					var res = await Task.Run(() => File.ReadAllText(path).FromJson<T>());
					_logger.Info("'{0}' has been read.", path);
					return res;
				}

				_logger.Warn("Can't read '{0}'. File does not exist.", path);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Can't read '{0}'.", path);
			}

			return default;
		}

		private List<T> ReadList<T>(string key = null)
		{
			string path = null;

			try
			{
				path = GetPath<T>(key);

				_logger.Info("Started reading '{0}'.", path);

				if (File.Exists(path))
				{
					var res = File.ReadAllText(path).FromJson<List<T>>();
					_logger.Info("'{0}' has been read.", path);
					return res;
				}

				_logger.Warn("Can't read '{0}'. File does not exist.", path);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Can't read '{0}'.", path);
			}

			return default;
		}

		private async Task<List<T>> ReadListAsync<T>(string key = null)
		{
			string path = null;

			try
			{
				path = GetPath<T>(key);

				_logger.Info("Started reading '{0}'.", path);

				if (File.Exists(path))
				{
					var res = await Task.Run(() => File.ReadAllText(path).FromJson<List<T>>());
					_logger.Info("'{0}' has been read.", path);
					return res;
				}

				_logger.Warn("Can't read '{0}'. File does not exist.", path);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Can't read '{0}'.", path);
			}

			return default;
		}

		private void Write<T>(T data, string key = null)
		{
			string path = null;

			try
			{
				lock (_locker)
				{
					path = GetPath<T>(key);

					_logger.Info("Started writing data to '{0}'.", path);

					if (!Directory.Exists(DataPath))
						Directory.CreateDirectory(DataPath);

					File.WriteAllText(path, data.ToJson());
					_logger.Info("Data has been written to '{0}'.", path);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Can't write data to '{0}'.", path);
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

		private async Task InitGroupsAsync()
		{
			var groups = await ReadListAsync<Group>();

			if (groups?.Any() == true)
				Groups = new ObservableCollection<Group>(groups);
			else
				Groups = new ObservableCollection<Group>();
		}

		private async Task InitMaxHoursAsync()
		{
			var academicStatuses = await ReadListAsync<AcademicStatusInfo>();

			if (academicStatuses == null)
			{
				ExtractJsonResource<AcademicStatusInfo>();
				academicStatuses = await ReadListAsync<AcademicStatusInfo>();
			}

			if (academicStatuses == null)
			{
				throw new Exception("Не вдалось ініціалізувати вчені ступені");
			}

			AcademicStatuses = new ObservableCollection<AcademicStatusInfo>(academicStatuses);
			AcademicStatuses.CollectionChanged += OnAcademicStatusesInfoChanged;

			foreach (var status in AcademicStatuses)
			{
				status.PropertyChanged += OnAcademicStatusInfoChanged;
			}
		}

		private async Task InitTimeNormsAsync()
		{
			var timeNorms = await ReadListAsync<TimeNorm>();

			if (timeNorms == null)
			{
				ExtractJsonResource<TimeNorm>();
				timeNorms = await ReadListAsync<TimeNorm>();
			}

			if (timeNorms == null)
			{
				throw new Exception("Не вдалось ініціалізувати часові норми");
			}

			TimeNorms = new ObservableCollection<TimeNorm>(timeNorms);
			TimeNorms.CollectionChanged += OnTimeNormsChanged;

			foreach (var timeNorm in TimeNorms)
			{
				timeNorm.PropertyChanged += OnTimeNormChanged;
			}
		}

		private void ExtractJsonResource<T>()
		{
			var key = GetKey<T>();

			using (FileStream fileStream = File.Create($@"{DataPath}\{key}.json"))
			{
				var resName = $"KafedraApp.Resources.Json.{key}.json";

				Assembly.GetExecutingAssembly()
					.GetManifestResourceStream(resName)
					.CopyTo(fileStream);
			}
		}

		private List<LoadItem> GetLoadItemsByTeacher(Subject subject)
		{
			var groups = GetGroupsBySpecAndCourse(subject.Specialty, subject.Course);
			var loadItems = new List<LoadItem>();

			foreach (var group in groups)
			{
				if (subject.LectureHours > 0)
				{
					var loadItem = GetLoadItem(
						subject,
						LoadItemTypes.Lectures,
						subject.LectureHours,
						group);

					loadItems.Add(loadItem);
				}

				if (subject.PracticalWorkHours > 0)
				{
					if (group.SubgroupsCount > 1)
					{
						for (int i = 1; i <= group.SubgroupsCount; ++i)
						{
							var loadItem = GetLoadItem(
								subject,
								LoadItemTypes.PracticalWork,
								subject.PracticalWorkHours,
								group,
								i);

							loadItems.Add(loadItem);
						}
					}
					else
					{
						var loadItem = GetLoadItem(
						subject,
						LoadItemTypes.PracticalWork,
						subject.PracticalWorkHours,
						group);

						loadItems.Add(loadItem);
					}
				}

				if (subject.LaboratoryWorkHours > 0)
				{
					if (group.SubgroupsCount > 1)
					{
						for (int i = 1; i <= group.SubgroupsCount; ++i)
						{
							var loadItem = GetLoadItem(
								subject,
								LoadItemTypes.LaboratoryWork,
								subject.LaboratoryWorkHours,
								group,
								i);

							loadItems.Add(loadItem);
						}
					}
					else
					{
						var loadItem = GetLoadItem(
						subject,
						LoadItemTypes.LaboratoryWork,
						subject.LaboratoryWorkHours,
						group);

						loadItems.Add(loadItem);
					}
				}

				if (subject.TestHours > 0)
				{
					var timeNorm = TimeNorms.FirstOrDefault(x => x.WorkType == WorkTypes.Test);

					if (timeNorm != null && timeNorm.Hours > 0)
					{
						var loadItem = GetLoadItem(
							subject,
							LoadItemTypes.Test,
							timeNorm.Hours,
							group);

						loadItems.Add(loadItem);
					}
				}

				if (subject.ExamHours > 0)
				{
					var timeNorm = TimeNorms.FirstOrDefault(x => x.WorkType == WorkTypes.Exam);

					if (timeNorm != null && timeNorm.Hours > 0)
					{
						var loadItem = GetLoadItem(
							subject,
							LoadItemTypes.Exam,
							timeNorm.Hours,
							group);

						loadItems.Add(loadItem);
					}
				}

				if (subject.IndividualTasksHours > 0)
				{
					var loadItem = GetLoadItem(
						subject,
						LoadItemTypes.IndividualTasks,
						subject.IndividualTasksHours,
						group);

					loadItems.Add(loadItem);
				}
			}

			return loadItems;
		}

		private LoadItem GetLoadItem(Subject subject, LoadItemTypes type, double hours, Group group, double subgroup = 0)
		{
			return new LoadItem
			{
				Subject = subject.Name,
				Semester = subject.Semester,
				Group = group.Name,
				Type = type,
				Hours = hours,
				Subgroup = subgroup
			};
		}

		private List<Subject> GetCathedraSubjects()
		{
			var assignedSubjects = Teachers.SelectMany(x => x.SubjectsSpecializesIn);
			var subjects = Subjects.Where(x => assignedSubjects.Contains(x.Name));
			return subjects.ToList();
		}

		private List<Group> GetGroupsBySpecAndCourse(string specialty, double course)
		{
			var groups = Groups
				.Where(x => x.Specialty == specialty && x.Course == course).ToList();

			return groups;
		}

		private void CopyDataTo<T>(string folder, string key = null)
		{
			var path = GetPath<T>(key);

			if (File.Exists(path))
			{
				File.Copy(GetPath<T>(), GetPath<T>(folder), true);
			}
		}

		#endregion

		#region Public Methods

		public async Task InitAsync()
		{
			await InitSubjectsAsync();
			await InitTeachersAsync();
			await InitGroupsAsync();
			await InitMaxHoursAsync();
			await InitTimeNormsAsync();
		}

		public async Task SaveTeachers()
		{
			await WriteAsync(Teachers);
		}

		public async Task SaveSubjects()
		{
			await WriteAsync(Subjects);
		}

		public async Task SaveGroups()
		{
			await WriteAsync(Groups);
		}

		public List<LoadItem> GetLoadItems(IEnumerable<Subject> subjects = null)
		{
			if (subjects?.Any() != true)
			{
				subjects = GetCathedraSubjects();
			}

			var loadItems = new List<LoadItem>();

			foreach (var subject in subjects)
			{
				loadItems.AddRange(GetLoadItemsByTeacher(subject));
			}

			return loadItems;
		}

		public void SetDataPath(string path)
		{
			DataPath = path;
			Settings.Default.DataPath = path;
		}

		public void CopyDataTo(string folder)
		{
			CopyDataTo<Subject>(folder);
			CopyDataTo<Teacher>(folder);
			CopyDataTo<Group>(folder);
			CopyDataTo<AcademicStatusInfo>(folder);
			CopyDataTo<TimeNorm>(folder);
		}

		#endregion

		#region Event Handlers

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

		private async void OnTimeNormsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			await WriteAsync(TimeNorms);
		}

		private async void OnTimeNormChanged(object sender, PropertyChangedEventArgs e)
		{
			await WriteAsync(TimeNorms);
		}

		#endregion
	}
}
